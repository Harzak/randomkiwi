using randomkiwi.Services.Http;

namespace randomkiwi.Factories;

/// <summary>
/// Provides functionality to create and configure instances of <see cref="HttpClientOption"/> with predefined settings.
/// </summary>
internal sealed class HttpClientOptionFactory : IHttpClientOptionFactory
{
    private readonly IWikipediaUrlBuilder _urlBuilder;
    private readonly IAppSettingsProvider _settingsProvider;

    public HttpClientOptionFactory(IWikipediaUrlBuilder urlBuilder, IAppSettingsProvider settingsProvider)
    {
        _urlBuilder = urlBuilder;
        _settingsProvider = settingsProvider;
    }

    public HttpClientOption CreateOption()
    {
        return new HttpClientOption(_settingsProvider.HttpClient.DefaultClientName, _settingsProvider.HttpClient.DefaultClientName)
        {
            BaseAddress = _urlBuilder.BuildBaseUri(),
        };
    }
}
