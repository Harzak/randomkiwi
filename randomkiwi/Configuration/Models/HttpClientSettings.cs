namespace randomkiwi.Configuration.Models;

/// <summary>
/// Configuration settings for HTTP client operations.
/// </summary>
public class HttpClientSettings
{
    public const string SectionName = "HttpClient";

    /// <summary>
    /// Gets or sets the default HTTP client name.
    /// </summary>
    public string DefaultClientName { get; set; } = string.Empty;
}