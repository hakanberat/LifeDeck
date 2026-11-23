
using System.Text.Json.Serialization;

namespace LifeDeck.Models;

public sealed record CurrencyRate
{
    [JsonPropertyName("date")]
    public DateOnly Date { get; init; }

    [JsonPropertyName("base")]
    public string Base { get; init; } = string.Empty;

    [JsonPropertyName("quote")]
    public string Quote { get; init; } = string.Empty;

    [JsonPropertyName("rate")]
    public decimal Rate { get; init; }
}
