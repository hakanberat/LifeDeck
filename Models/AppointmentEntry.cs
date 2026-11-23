
namespace LifeDeck.Models;

public sealed record AppointmentEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Today;

    public string Time { get; set; } = "09:00";

    public int DurationMinutes { get; set; } = 60;

    public string Type { get; set; } = "Personal";

    public string Location { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
        = DateTimeOffset.UtcNow;
}
