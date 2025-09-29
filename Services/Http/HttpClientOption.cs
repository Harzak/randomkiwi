using randomkiwi.Constants;
using randomkiwi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Services.Http;

/// <summary>
/// Defines the contract for HTTP client configuration options including base address and identification properties.
/// </summary>
public record HttpClientOption : IHttpClientOption
{
    /// <inheritdoc />
    public required Uri BaseAddress { get; init; }

    /// <inheritdoc />
    public string Name { get; init; }

    /// <inheritdoc />
    public string UserAgent { get; init; }

    public HttpClientOption(string name)
    {
        this.Name = name;
        this.UserAgent = HttpClientConsts.HTTPCLIENT_NAME_DEFAULT;
    }
}