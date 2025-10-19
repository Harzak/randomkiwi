using Microsoft.Extensions.Options;

namespace randomkiwi.Configuration;

/// <summary>
/// Provides access to strongly-typed configuration settings.
/// </summary>
internal sealed class AppSettingsProvider : IAppSettingsProvider
{
    /// <inheritdoc/>
    public AppSettings App { get; }

    /// <inheritdoc/>
    public UserMetricsSettings UserMetrics { get; }

    /// <inheritdoc/>
    public ArticleCatalogSettings ArticleCatalog { get; }

    /// <inheritdoc/>
    public HttpClientSettings HttpClient { get; }

    /// <inheritdoc/>
    public WikipediaSettings Wikipedia { get; }

    public AppSettingsProvider(
        IOptions<AppSettings> appSettings,
        IOptions<UserMetricsSettings> userMetricsSettings,
        IOptions<ArticleCatalogSettings> articleCatalogSettings,
        IOptions<HttpClientSettings> httpClientSettings,
        IOptions<WikipediaSettings> wikipediaSettings)
    {
        App = appSettings.Value;
        UserMetrics = userMetricsSettings.Value;
        ArticleCatalog = articleCatalogSettings.Value;
        HttpClient = httpClientSettings.Value;
        Wikipedia = wikipediaSettings.Value;
    }
}