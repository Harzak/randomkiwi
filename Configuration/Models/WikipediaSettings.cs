using System.Text;

namespace randomkiwi.Configuration.Models;

/// <summary>
/// Configuration settings for Wikipedia API integration.
/// </summary>
public class WikipediaSettings
{
    public const string SectionName = "Wikipedia";

    /// <summary>
    /// Gets or sets the URL format template for Wikipedia subdomains.
    /// </summary>
    public string UrlFormat { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the query endpoint format template for the MediaWiki API.
    /// </summary>
    public string QueryEndpointFormat { get; set; } = string.Empty;

    /// <summary>
    /// Gets the URL format as a CompositeFormat for string formatting.
    /// </summary>
    public CompositeFormat GetUrlCompositeFormat() => CompositeFormat.Parse(UrlFormat);

    /// <summary>
    /// Gets the query endpoint format as a CompositeFormat for string formatting.
    /// </summary>
    public CompositeFormat GetQueryEndpointCompositeFormat() => CompositeFormat.Parse(QueryEndpointFormat);
}