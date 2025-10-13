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

    public HttpClientOption(string name, string userAgent)
    {
        this.Name = name;
        this.UserAgent = userAgent;
    }
}