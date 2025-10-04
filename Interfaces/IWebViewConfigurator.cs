using System.Net;

namespace randomkiwi.Interfaces;

/// <summary>
/// Provides configuration options for a web view
/// </summary>
public interface IWebViewConfigurator
{
    /// <summary>
    /// Creates and initializes a <see cref="CookieContainer"/> with a theme-related cookie for the specified target URI.
    /// </summary>
    CookieContainer? CreateCookieContainer(Uri targetUri);
}