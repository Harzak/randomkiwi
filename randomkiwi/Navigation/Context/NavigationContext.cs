namespace randomkiwi.Navigation.Guards;

/// <summary>
/// Navigation context that includes parameters and metadata
/// </summary>
public sealed class NavigationContext
{
    public NavigationParameters Parameters { get; }
    public string? SourceView { get; }
    public DateTime NavigatedAt { get; }

    public NavigationContext(NavigationParameters? parameters = null, string? sourceView = null)
    {
        Parameters = parameters ?? [];
        SourceView = sourceView;
        NavigatedAt = DateTime.UtcNow;
    }
}

