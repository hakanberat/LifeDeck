
namespace LifeDeck.Models;

public sealed record TodoItem
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Text { get; init; } = string.Empty;

    public bool IsCompleted { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
        = DateTimeOffset.UtcNow;
}
