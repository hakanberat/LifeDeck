
using System.Text.Json;
using LifeDeck.Models;
using LifeDeck.Services.Storage;
using Microsoft.JSInterop;
using System.Text;



namespace LifeDeck.Services.Backup;

public sealed class BackupService : IAsyncDisposable
{
    private readonly BrowserStorageService _storage;
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    public BackupService(
        BrowserStorageService storage,
        IJSRuntime js)
    {
        _storage = storage;
        _js = js;
    }

    public async Task<string> SaveJsonAsync()
    {
        // Read actual records from every module.
        var backup = await CreateBackupAsync();

        // Serialize the backup into a JSON document.
        var json = JsonSerializer.Serialize(
            backup,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        var fileName =
            $"LifeDeck-Backup-{DateTime.Now:yyyy-MM-dd-HHmmss}.json";

        // JavaScript is used only for browser file access.
        _module ??= await _js.InvokeAsync<IJSObjectReference>(
            "import",
            "./js/backup.js");

        return await _module.InvokeAsync<string>(
            "saveJson",
            json,
            fileName);
    }

    private async Task<List<JsonElement>> ReadAsync(string key)
    {
        return await _storage.GetAsync<List<JsonElement>>(key)
            ?? new List<JsonElement>();
    }

        
private async Task<LifeDeckBackup> CreateBackupAsync()
{
    var data = new Dictionary<string, List<JsonElement>>
    {
        ["dives"] = await ReadAsync(StorageKeys.Dives),
        ["todos"] = await ReadAsync(StorageKeys.Todos),
        ["notes"] = await ReadAsync(StorageKeys.Notes),
        ["appointments"] = await ReadAsync(StorageKeys.Appointments),
        ["transactions"] = await ReadAsync(StorageKeys.Transactions),
        ["blogPosts"] = await ReadAsync(StorageKeys.BlogPosts)
    };

    return new LifeDeckBackup
    {
        Data = data
    };
}

public async Task<string> SaveTxtAsync()
{
    var backup = await CreateBackupAsync();

    var builder = new StringBuilder();

    builder.AppendLine("LIFEDECK DATA EXPORT");
    builder.AppendLine("====================");
    builder.AppendLine(
        $"Created: {backup.CreatedAtUtc:yyyy-MM-dd HH:mm:ss} UTC");
    builder.AppendLine();

    var modules = new (string Name, string Key)[]
    {
        ("LOGBOOK", "dives"),
        ("TODO", "todos"),
        ("NOTEBOOK", "notes"),
        ("CALENDAR", "appointments"),
        ("PERSONAL ACCOUNT", "transactions"),
        ("BLOG", "blogPosts")
    };

    foreach (var (name, key) in modules)
    {
        var records = backup.Data[key];

        builder.AppendLine($"========== {name} ==========");
        builder.AppendLine($"Records: {records.Count}");
        builder.AppendLine();

        for (int i = 0; i < records.Count; i++)
        {
            builder.AppendLine($"--- Record {i + 1} ---");

            foreach (var property in records[i].EnumerateObject())
            {
                var value = property.Value.ValueKind switch
                {
                    JsonValueKind.String =>
                        property.Value.GetString() ?? "",

                    JsonValueKind.Null => "",

                    JsonValueKind.Array =>
                        string.Join(", ",
                            property.Value.EnumerateArray()
                                .Select(x => x.ToString())),

                    _ => property.Value.ToString()
                };

                builder.AppendLine(
                    $"{property.Name}: {value}");
            }

            builder.AppendLine();
        }
    }

    return await SaveFileAsync(
        Encoding.UTF8.GetBytes(builder.ToString()),
        "txt",
        "text/plain;charset=utf-8");
}

public async Task<string> SaveExcelAsync()
{
    var backup = await CreateBackupAsync();

    byte[] content = ExcelBackupWriter.Create(backup);

    return await SaveFileAsync(
        content,
        "xlsx",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
}

private async Task<string> SaveFileAsync(
    byte[] content,
    string extension,
    string mimeType)
{
    var fileName =
        $"LifeDeck-Export-{DateTime.Now:yyyy-MM-dd-HHmmss}.{extension}";

    _module ??= await _js.InvokeAsync<IJSObjectReference>(
        "import",
        "./js/backup.js");

    return await _module.InvokeAsync<string>(
        "saveFile",
        content,
        fileName,
        mimeType,
        extension);
}


        
private static readonly (string BackupKey, string StorageKey)[] RestoreKeys =
{
    ("dives", StorageKeys.Dives),
    ("todos", StorageKeys.Todos),
    ("notes", StorageKeys.Notes),
    ("appointments", StorageKeys.Appointments),
    ("transactions", StorageKeys.Transactions),
    ("blogPosts", StorageKeys.BlogPosts)
};

// Validate a backup before changing any saved data.
public LifeDeckBackup ValidateBackup(string json)
{
    using var document = JsonDocument.Parse(json);
    var root = document.RootElement;

    if (root.ValueKind != JsonValueKind.Object ||
        !root.TryGetProperty("Format", out var format) ||
        format.ValueKind != JsonValueKind.String ||
        format.GetString() != "LifeDeck" ||
        !root.TryGetProperty("Version", out var version) ||
        version.ValueKind != JsonValueKind.Number ||
        !version.TryGetInt32(out var versionNumber) ||
        versionNumber != 1 ||
        !root.TryGetProperty("CreatedAtUtc", out var created) ||
        created.ValueKind != JsonValueKind.String ||
        !created.TryGetDateTimeOffset(out _) ||
        !root.TryGetProperty("Data", out var data) ||
        data.ValueKind != JsonValueKind.Object)
    {
        throw new InvalidDataException(
            "Invalid or unsupported LifeDeck backup.");
    }

    var backup = JsonSerializer.Deserialize<LifeDeckBackup>(json)
        ?? throw new InvalidDataException("Backup is empty.");

    if (backup.Data is null ||
        backup.Data.Count != RestoreKeys.Length)
    {
        throw new InvalidDataException(
            "Backup modules are missing or invalid.");
    }

    foreach (var (backupKey, _) in RestoreKeys)
    {
        if (!backup.Data.TryGetValue(backupKey, out var rows) ||
            rows is null)
        {
            throw new InvalidDataException(
                $"Missing module: {backupKey}");
        }

        switch (backupKey)
        {
            case "dives":
                ValidateRecords<DiveEntry>(
                    rows, backupKey, "Date", "Location");
                break;

            case "todos":
                ValidateRecords<TodoItem>(
                    rows, backupKey, "Text", "IsCompleted");
                break;

            case "notes":
                ValidateRecords<NoteEntry>(
                    rows, backupKey, "Title", "Content");
                break;

            case "appointments":
                ValidateRecords<AppointmentEntry>(
                    rows, backupKey, "Title", "Date", "Time");
                break;

            case "transactions":
                ValidateRecords<TransactionEntry>(
                    rows, backupKey,
                    "Date", "Type", "Amount", "Currency");
                break;

            case "blogPosts":
                ValidateRecords<BlogPost>(
                    rows, backupKey, "Title", "Content", "Tags");
                break;
        }
    }

    return backup;
}

private static void ValidateRecords<T>(
    List<JsonElement> rows,
    string moduleName,
    params string[] requiredProperties)
{
    var ids = new HashSet<Guid>();

    foreach (var row in rows)
    {
        if (row.ValueKind != JsonValueKind.Object ||
            !row.TryGetProperty("Id", out var idValue) ||
            idValue.ValueKind != JsonValueKind.String ||
            !Guid.TryParse(idValue.GetString(), out var id) ||
            id == Guid.Empty ||
            !ids.Add(id))
        {
            throw new InvalidDataException(
                $"Invalid or duplicate record ID in {moduleName}.");
        }

        foreach (var property in requiredProperties)
        {
            if (!row.TryGetProperty(property, out _))
            {
                throw new InvalidDataException(
                    $"Missing {property} in {moduleName}.");
            }
        }

        try
        {
            if (row.Deserialize<T>() is null)
            {
                throw new InvalidDataException(
                    $"Invalid record in {moduleName}.");
            }
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException(
                $"Invalid record format in {moduleName}.", ex);
        }
    }
}

// Replace local data with validated backup records.
public async Task<string> RestoreAsync(LifeDeckBackup backup)
{
    // Validate again immediately before writing.
    ValidateBackup(JsonSerializer.Serialize(backup));

    var dataToRestore = new Dictionary<string, string>();

    foreach (var (backupKey, storageKey) in RestoreKeys)
    {
        dataToRestore[storageKey] =
            JsonSerializer.Serialize(backup.Data[backupKey]);
    }

    _module ??= await _js.InvokeAsync<IJSObjectReference>(
        "import", "./js/backup.js");

    return await _module.InvokeAsync<string>(
        "restoreData",
        dataToRestore);
}

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
            await _module.DisposeAsync();
    }
}
