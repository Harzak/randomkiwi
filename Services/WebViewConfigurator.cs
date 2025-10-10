using System.Globalization;
using System.Net;

namespace randomkiwi.Services;

/// <summary>
/// Provides configuration utilities for setting up a web view.
/// </summary>
public sealed class WebViewConfigurator : IWebViewConfigurator
{
    private readonly IAppConfiguration _appConfig;

    public WebViewConfigurator(IAppConfiguration appConfig)
    {
        _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
    }

    /// <inheritdoc/>
    public CookieContainer? CreateCookieContainer(Uri targetUri)
    {
        ArgumentNullException.ThrowIfNull(targetUri);

        CookieContainer cookieContainer = new();
        string languageCode = _appConfig.LanguageCode;
        string themeCode = _appConfig.GetEffectiveThemeCode() == AppTheme.Dark ? WikipediaWebConsts.COOKIE_THEME_CLIENT_VALUE_NIGHT : WikipediaWebConsts.COOKIE_THEME_CLIENT_VALUE_LIGHT;

        Cookie cookie = new()
        {
            Name = String.Format(CultureInfo.InvariantCulture, WikipediaWebConsts.COOKIE_THEME_CLIENT_KEY_FORMAT, languageCode),
            Expires = DateTime.Now.AddDays(1),
            Value = String.Format(CultureInfo.InvariantCulture, WikipediaWebConsts.COOKIE_THEME_CLIENT_VALUE_FORMAT, themeCode),
            Domain = targetUri.Host,
            Path = "/"
        };

        try
        {
            cookieContainer.Add(targetUri, cookie);
        }
        catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
        {
            return null;
        }
        catch (Exception)
        {
            throw;
        }

        return cookieContainer;
    }
}
