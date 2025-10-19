using System.Globalization;

namespace randomkiwi.Services.Http;

/// <inheritdoc />
internal sealed class WikipediaUrlBuilder : IWikipediaUrlBuilder
{
    private readonly IAppConfiguration _appConfiguration;
    private readonly IAppSettingsProvider _settingsProvider;

    public WikipediaUrlBuilder(IAppConfiguration appConfiguration, IAppSettingsProvider settingsProvider)
    {
        _appConfiguration = appConfiguration;
        _settingsProvider = settingsProvider;
    }

    /// <inheritdoc />
    public Uri BuildBaseUri()
    {
        string uriStr = string.Format(CultureInfo.InvariantCulture, _settingsProvider.Wikipedia.UrlFormat, _appConfiguration.LanguageCode);
        return new Uri(uriStr);
    }

    /// <inheritdoc />
    public Uri BuildArticleUrl(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Article title cannot be null or empty.", nameof(title));
        }

        string formattedTitle = title.Replace(" ", "_", StringComparison.InvariantCulture);
        string encodedTitle = Uri.EscapeDataString(formattedTitle);
        return new Uri(this.BuildBaseUri(), $"/wiki/{encodedTitle}");
    }
}