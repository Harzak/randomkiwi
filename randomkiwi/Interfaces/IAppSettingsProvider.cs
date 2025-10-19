namespace randomkiwi.Interfaces;

/// <summary>
/// Provides centralized access to application configuration settings.
/// </summary>
public interface IAppSettingsProvider
{
    /// <summary>
    /// Gets the application settings.
    /// </summary>
    AppSettings App { get; }

    /// <summary>
    /// Gets the user metrics settings.
    /// </summary>
    UserMetricsSettings UserMetrics { get; }

    /// <summary>
    /// Gets the article catalog settings.
    /// </summary>
    ArticleCatalogSettings ArticleCatalog { get; }

    /// <summary>
    /// Gets the HTTP client settings.
    /// </summary>
    HttpClientSettings HttpClient { get; }

    /// <summary>
    /// Gets the Wikipedia settings.
    /// </summary>
    WikipediaSettings Wikipedia { get; }
}