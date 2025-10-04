using System.Text;

namespace randomkiwi.Constants;

internal static class WikipediaEndpoint
{
    internal static readonly CompositeFormat URL_FORMAT = CompositeFormat.Parse("https://{0}.m.wikipedia.org");
    internal static readonly CompositeFormat ENDPOINT_FORMAT_QUERY_PAGEPROPS = CompositeFormat.Parse("/w/api.php?action=query&generator=random&grnnamespace={0}&grnlimit={1}&prop=pageprops&format=json");
}
