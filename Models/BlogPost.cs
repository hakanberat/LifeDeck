
namespace LifeDeck.Models;

public sealed record BlogPost
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Title { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public List<string> Tags { get; init; } = new();

    public DateTimeOffset CreatedAt { get; init; }
        = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; init; }
        = DateTimeOffset.UtcNow;
}
