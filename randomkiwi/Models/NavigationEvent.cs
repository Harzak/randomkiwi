namespace randomkiwi.Models;

/// <summary>
/// Represents a navigation event in the user's session
/// </summary>
public sealed record NavigationEvent
{
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public ENavigationType Type { get; init; }
    public int ToArticleId { get; init; }
}

