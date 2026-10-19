
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using LifeDeck.Models;

namespace LifeDeck.Services.Backup;

public static class ExcelBackupWriter
{
    private static readonly XNamespace Main =
        "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

    private static readonly XNamespace OfficeRel =
        "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

    private static readonly XNamespace PackageRel =
        "http://schemas.openxmlformats.org/package/2006/relationships";

    private static readonly XNamespace ContentTypes =
        "http://schemas.openxmlformats.org/package/2006/content-types";

    private sealed record SheetDefinition(
        string Name,
        string Key,
        string[] Columns);

    private static readonly SheetDefinition[] Sheets =
    {
        new("Logbook", "dives", new[]
        {
            "Id", "Date", "Location", "DiveSite",
            "MaxDepthMeters", "BottomTimeMinutes",
            "DiveType", "Tank", "SuitType", "SuitLength",
            "SuitThicknessMm", "WeightKg", "Buddy",
            "Notes", "CreatedAt"
        }),

        new("Todo", "todos", new[]
        {
            "Id", "Text", "IsCompleted", "CreatedAt"
        }),

        new("Notebook", "notes", new[]
        {
            "Id", "Title", "Content", "CreatedAt",
            "UpdatedAt"
        }),

        new("Calendar", "appointments", new[]
        {
            "Id", "Title", "Date", "Time",
            "DurationMinutes", "Type", "Location",
            "Description", "CreatedAt"
        }),

        new("Transactions", "transactions", new[]
        {
            "Id", "Date", "Type", "Amount",
            "Currency", "Category", "PaymentMethod",
            "Description", "CreatedAt"
        }),

        new("Blog", "blogPosts", new[]
        {
            "Id", "Title", "Content", "Tags",
            "CreatedAt", "UpdatedAt"
        })
    };

    public static byte[] Create(LifeDeckBackup backup)
    {
        using var memory = new MemoryStream();

        using (var zip = new ZipArchive(
            memory,
            ZipArchiveMode.Create,
            leaveOpen: true))
        {
            CreateContentTypes(zip);
            CreatePackageRelationships(zip);
            CreateWorkbook(zip);
            CreateWorkbookRelationships(zip);

            for (int i = 0; i < Sheets.Length; i++)
            {
                CreateWorksheet(zip, backup, Sheets[i], i + 1);
            }
        }

        return memory.ToArray();
    }

    private static void CreateContentTypes(ZipArchive zip)
    {
        var root = new XElement(ContentTypes + "Types",
            new XElement(ContentTypes + "Default",
                new XAttribute("Extension", "rels"),
                new XAttribute("ContentType",
                    "application/vnd.openxmlformats-package.relationships+xml")),

            new XElement(ContentTypes + "Default",
                new XAttribute("Extension", "xml"),
                new XAttribute("ContentType", "application/xml")),

            new XElement(ContentTypes + "Override",
                new XAttribute("PartName", "/xl/workbook.xml"),
                new XAttribute("ContentType",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"))
        );

        for (int i = 1; i <= Sheets.Length; i++)
        {
            root.Add(new XElement(ContentTypes + "Override",
                new XAttribute(
                    "PartName", $"/xl/worksheets/sheet{i}.xml"),
                new XAttribute("ContentType",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml")));
        }

        AddXml(zip, "[Content_Types].xml", new XDocument(root));
    }

    private static void CreatePackageRelationships(ZipArchive zip)
    {
        var root = new XElement(PackageRel + "Relationships",
            new XElement(PackageRel + "Relationship",
                new XAttribute("Id", "rId1"),
                new XAttribute("Type",
    "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"),
                new XAttribute("Target", "xl/workbook.xml"))
        );

        AddXml(zip, "_rels/.rels", new XDocument(root));
    }

    private static void CreateWorkbook(ZipArchive zip)
    {
        var sheets = new XElement(Main + "sheets");

        for (int i = 0; i < Sheets.Length; i++)
        {
            sheets.Add(new XElement(Main + "sheet",
                new XAttribute("name", Sheets[i].Name),
                new XAttribute("sheetId", i + 1),
                new XAttribute(OfficeRel + "id", $"rId{i + 1}")));
        }

        var workbook = new XElement(Main + "workbook",
            new XAttribute(XNamespace.Xmlns + "r", OfficeRel),
            sheets);

        AddXml(zip, "xl/workbook.xml", new XDocument(workbook));
    }

    private static void CreateWorkbookRelationships(
        ZipArchive zip)
    {
        var root = new XElement(PackageRel + "Relationships");

        for (int i = 1; i <= Sheets.Length; i++)
        {
            root.Add(new XElement(PackageRel + "Relationship",
                new XAttribute("Id", $"rId{i}"),
                new XAttribute("Type",
    "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet"),
                new XAttribute("Target",
                    $"worksheets/sheet{i}.xml")));
        }

        AddXml(
            zip,
            "xl/_rels/workbook.xml.rels",
            new XDocument(root));
    }

    private static void CreateWorksheet(
        ZipArchive zip,
        LifeDeckBackup backup,
        SheetDefinition definition,
        int sheetNumber)
    {
        var rows = backup.Data[definition.Key];

        // Preserve additional properties if models evolve.
        var columns = definition.Columns
            .Concat(rows.SelectMany(row =>
                row.EnumerateObject().Select(p => p.Name)))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        var sheetData = new XElement(Main + "sheetData");

        var header = new XElement(
            Main + "row",
            new XAttribute("r", 1));

        for (int i = 0; i < columns.Length; i++)
        {
            header.Add(TextCell(
                $"{ColumnName(i)}1",
                columns[i]));
        }

        sheetData.Add(header);

        for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            int excelRow = rowIndex + 2;

            var row = new XElement(
                Main + "row",
                new XAttribute("r", excelRow));

            var record = rows[rowIndex];

            for (int columnIndex = 0;
                 columnIndex < columns.Length;
                 columnIndex++)
            {
                string reference =
                    $"{ColumnName(columnIndex)}{excelRow}";

                if (!record.TryGetProperty(
                    columns[columnIndex], out var value))
                {
                    continue;
                }

                row.Add(CreateCell(reference, value));
            }

            sheetData.Add(row);
        }

        var worksheet = new XElement(
            Main + "worksheet",
            sheetData);

        AddXml(
            zip,
            $"xl/worksheets/sheet{sheetNumber}.xml",
            new XDocument(worksheet));
    }

    private static XElement CreateCell(
        string reference,
        JsonElement value)
    {
        if (value.ValueKind == JsonValueKind.Number)
        {
            return new XElement(Main + "c",
                new XAttribute("r", reference),
                new XElement(Main + "v", value.GetRawText()));
        }

        if (value.ValueKind is JsonValueKind.True
            or JsonValueKind.False)
        {
            return new XElement(Main + "c",
                new XAttribute("r", reference),
                new XAttribute("t", "b"),
                new XElement(Main + "v",
                    value.GetBoolean() ? "1" : "0"));
        }

        string text = FormatValue(value);

        return TextCell(reference, text);
    }

    private static XElement TextCell(
        string reference,
        string text)
    {
        return new XElement(Main + "c",
            new XAttribute("r", reference),
            new XAttribute("t", "inlineStr"),
            new XElement(Main + "is",
                new XElement(Main + "t",
                    new XAttribute(
                        XNamespace.Xml + "space", "preserve"),
                    text)));
    }

    private static string FormatValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String =>
                value.GetString() ?? string.Empty,

            JsonValueKind.Null =>
                string.Empty,

            JsonValueKind.Array =>
                string.Join(", ",
                    value.EnumerateArray().Select(
                        item => item.ToString())),

            _ => value.ToString()
        };
    }

    private static string ColumnName(int index)
    {
        var name = new StringBuilder();
        index++;

        while (index > 0)
        {
            index--;
            name.Insert(0, (char)('A' + index % 26));
            index /= 26;
        }

        return name.ToString();
    }

    private static void AddXml(
        ZipArchive zip,
        string path,
        XDocument document)
    {
        var entry = zip.CreateEntry(
            path,
            CompressionLevel.Optimal);

        using var stream = entry.Open();
        document.Save(stream);
    }
}
