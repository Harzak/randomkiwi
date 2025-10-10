using System.Text;

namespace randomkiwi.Constants;

internal static class WikipediaEndpoint
{
    /// <summary>
    /// Represents the format string for constructing URLs to Wikipedia subdomains.
    /// </summary>
    internal static readonly CompositeFormat URL_FORMAT = CompositeFormat.Parse("https://{0}.m.wikipedia.org");

    /// <summary>
    /// Represents the format string for constructing a query to the MediaWiki API.
    /// </summary>
    internal static readonly CompositeFormat ENDPOINT_FORMAT_QUERY_PAGEPROPS 
        = CompositeFormat.Parse("/w/api.php" +
            "?action=query" +
            "&generator=random" +
            "&grnnamespace={0}" +
            "&grnlimit={1}" +
            "&prop=pageprops" +
            "&grnfilterredir=nonredirects" +
            "&prop=pageprops%7Cinfo"+
            "&inprop=length"+
            "&format=json");
}
