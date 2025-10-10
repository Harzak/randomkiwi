using System.Globalization;

namespace randomkiwi.Interfaces;


/// <summary>
/// Represents the configuration settings for the app.
/// </summary>
public interface IAppConfiguration
{
    /// <summary>
    /// Gets the collection of cultures supported by the application.
    /// </summary>
    IReadOnlyCollection<CultureInfo> SupportedCultures { get; }
    /// <summary>
    /// Gets or sets the culture information used for formatting and parsing operations.
    /// </summary>
    CultureInfo CurrentCulture { get; set; }
    /// <summary>
    /// Gets the collection of available themes that can be applied to the application.
    /// </summary>
    IReadOnlyCollection<AppTheme> AvailableThemes { get; }
    /// <summary>
    /// Gets or sets the current theme variant applied to the application.
    /// </summary>
    AppTheme CurrentTheme { get; set; }
    string LanguageCode { get; }
    /// <summary>
    /// Gets or sets the detail level about the article.
    /// </summary>
    EArticleDetail ArticleDetail { get; set; }
    Version AppVersion { get; }

    Task InitializeAsync();
    Task SaveAsync();

    /// <summary>
    /// Determines the effective theme to be used by the application.
    /// </summary>
    AppTheme GetEffectiveThemeCode();
}