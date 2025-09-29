using randomkiwi.Constants;
using randomkiwi.Interfaces;
using randomkiwi.Services.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Factories;

/// <summary>
/// Provides functionality to create and configure instances of <see cref="HttpClientOption"/> with predefined settings.
/// </summary>
internal sealed class HttpClientOptionFactory : IHttpClientOptionFactory
{
    private readonly IWikipediaUrlBuilder _urlBuilder;

    public HttpClientOptionFactory(IWikipediaUrlBuilder  urlBuilder)
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
