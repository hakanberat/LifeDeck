
using System.Text.Json;

namespace LifeDeck.Models;

public sealed record LifeDeckBackup
{
    public string Format { get; init; } = "LifeDeck";

    public int Version { get; init; } = 1;

    public DateTimeOffset CreatedAtUtc { get; init; }
        = DateTimeOffset.UtcNow;

    public Dictionary<string, List<JsonElement>> Data { get; init; }
        = new();
}
