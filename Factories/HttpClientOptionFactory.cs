using randomkiwi.Services.Http;

namespace randomkiwi.Factories;

/// <summary>
/// Provides functionality to create and configure instances of <see cref="HttpClientOption"/> with predefined settings.
/// </summary>
internal sealed class HttpClientOptionFactory : IHttpClientOptionFactory
{
    private readonly IWikipediaUrlBuilder _urlBuilder;

    public HttpClientOptionFactory(IWikipediaUrlBuilder urlBuilder)
    {
        _urlBuilder = urlBuilder;
    }

    public HttpClientOption CreateOption()
    {
        return new HttpClientOption(HttpClientConsts.HTTPCLIENT_NAME_DEFAULT)
        {
            BaseAddress = _urlBuilder.BuildBaseUri(),
        };
    }
}
