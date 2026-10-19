
namespace LifeDeck.Models;

public sealed record TransactionEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime Date { get; set; } = DateTime.Today;

    public string Type { get; set; } = "Expense";

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "TRY";

    public string Category { get; set; } = string.Empty;

    public string PaymentMethod { get; set; } = "Cash";

    public string Description { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
        = DateTimeOffset.UtcNow;
}
