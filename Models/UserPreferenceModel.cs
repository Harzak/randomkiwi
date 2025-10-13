namespace randomkiwi.Models;

/// <summary>
/// Represents a user's application preferences.
/// </summary>
public sealed record UserPreferenceModel
{
    public required string AppLanguage { get; init; }
    public required AppTheme Theme { get; init; }
    public required EArticleDetail ArticleDetail { get; init; }
}