
namespace LifeDeck.Models;

public sealed record DiveEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime Date { get; set; } = DateTime.Today;

    public string Location { get; set; } = string.Empty;

    public string DiveSite { get; set; } = string.Empty;

    public decimal MaxDepthMeters { get; set; }

    public int? BottomTimeMinutes { get; set; }

    public string DiveType { get; set; } = "Recreational";

    public string Tank { get; set; } = string.Empty;

    public string SuitType { get; set; } = "None";

    public string SuitLength { get; set; } = string.Empty;

    public decimal? SuitThicknessMm { get; set; }

    public decimal? WeightKg { get; set; }

    public string Buddy { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
        = DateTimeOffset.UtcNow;
}
