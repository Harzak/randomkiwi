using randomkiwi.Constants;
using randomkiwi.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Services.Http;

/// <inheritdoc />
internal sealed class WikipediaUrlBuilder : IWikipediaUrlBuilder
{
    private readonly IAppConfiguration _appConfiguration;

    public WikipediaUrlBuilder(IAppConfiguration appConfiguration)
    {
        _appConfiguration = appConfiguration;
    }

    /// <inheritdoc />
    public Uri BuildBaseUri()
    {
        string uriStr = string.Format(CultureInfo.InvariantCulture, WikipediaEndpoint.URL_FORMAT, _appConfiguration.LanguageCode);
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