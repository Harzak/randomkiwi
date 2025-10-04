using Microsoft.Extensions.Logging;
using randomkiwi.Constants;
using randomkiwi.Factories;
using randomkiwi.Interfaces;
using randomkiwi.Services;
using randomkiwi.Services.Http;
using randomkiwi.Views;
using randomkiwi.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Configuration;


/// <summary>
/// Extension methods for configuring core services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds core application services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <remarks>
    /// This method registers essential services including the main view model, navigation service,
    /// and view model factory required for the core application functionality.
    /// </remarks>
    public static void AddCoreServices(this IServiceCollection collection)
    {
        collection.AddHttpClient();

        // ViewModels
        collection.AddSingleton<MainViewModel>();
        collection.AddSingleton<SettingsViewModel>();
        collection.AddSingleton<BookmarksViewModel>();
        collection.AddTransient<WikipediaWebViewViewModel>();

        // Services
        collection.AddSingleton<IArticleCatalog, WikipediaArticleCatalog>();
        collection.AddSingleton<IWikipediaAPIClient, WikipediaAPIClient>();
        collection.AddSingleton<IWikipediaUrlBuilder, WikipediaUrlBuilder>();
        collection.AddSingleton<IUserMetricsService, UserMetricsService>();
        collection.AddSingleton<IWebViewConfigurator, WebViewConfigurator>();
        collection.AddTransient<IWebViewManager, WebViewManager>();
        collection.AddSingleton<IScriptLoader, ScriptLoader>();

        collection.AddSingleton<IHttpClientOptionFactory, HttpClientOptionFactory>();
        collection.AddSingleton<IAppConfiguration, AppConfiguration>();
        collection.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        collection.AddSingleton<Func<int, IDebounceAction>>(serviceProvider =>
        {
            return delayMs => new DebounceAction(delayMs);
        });
    }

    private static void AddHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient(HttpClientConsts.HTTPCLIENT_NAME_DEFAULT)
            .ConfigurePrimaryHttpMessageHandler(CreatePlatformOptimizedHandler);
    }

    private static HttpMessageHandler CreatePlatformOptimizedHandler()
    {

#if IOS || MACCATALYST
        return new NSUrlSessionHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            UseCookies = true,
            CookieContainer = new CookieContainer(),
            DisableCaching = false,
            AllowAutoRedirect = true,
            MaxAutomaticRedirections = 5,
            MaxConnectionsPerServer = 6,
            PreAuthenticate = false
        };

#elif WINDOWS
        return new WinHttpHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
            CookieUsePolicy = CookieUsePolicy.UseInternalCookieStoreOnly,
            CookieContainer = new CookieContainer(),
            MaxConnectionsPerServer = 10,
            ReceiveDataTimeout = TimeSpan.FromSeconds(30),
            ReceiveHeadersTimeout = TimeSpan.FromSeconds(15),
            SendTimeout = TimeSpan.FromSeconds(30),
            WindowsProxyUsePolicy = WindowsProxyUsePolicy.UseWinHttpProxy,
            EnableMultipleHttp2Connections = true,
            MaxAutomaticRedirections = 5
        };

#else
        return new HttpClientHandler
        {
            // On Android, HttpClientHandler automatically uses the native Android implementation
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            UseCookies = true,
            CookieContainer = new CookieContainer(),
            MaxConnectionsPerServer = 10,
            AllowAutoRedirect = true,
            MaxAutomaticRedirections = 5,
            PreAuthenticate = false
        };
#endif
    }
}

