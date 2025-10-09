using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using randomkiwi.Navigation.ViewModels;
using randomkiwi.Navigation.WebPage;
using randomkiwi.Repositories;
using randomkiwi.Services.Http;
using System.Net;

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

        // Views
        collection.AddSingleton<MainView>();

        // ViewModels
        collection.AddSingleton<MainViewModel>();
        collection.AddSingleton<RandomWikipediaViewModel>();
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
        collection.AddSingleton<IUserPreferenceRepository, UserPreferenceRepository>();
        collection.AddSingleton<INavigationHandler<IRoutableViewModel>, ViewModelNavigationHandler>();
        collection.AddSingleton<INavigationHandler<IRoutableItem>, WebPageNavigationHandler>();
        collection.AddSingleton<IViewModelNavigationService, ViewModelNavigationService>();
        collection.AddSingleton<IWebPageNavigationService, WebPageNavigationService>();
        collection.AddSingleton<INavigationService, NavigationService>();
        collection.AddSingleton<IJsonStorage<UserPreferenceModel>>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<JsonStorage<UserPreferenceModel>>>();
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appDirectory = Path.Combine(appDataPath, AppConsts.APP_NAME);
            return new JsonStorage<UserPreferenceModel>(appDirectory, AppConsts.USER_PREFERENCES_FILE, logger);
        });
        collection.AddSingleton<IHttpClientOptionFactory, HttpClientOptionFactory>();
        collection.AddSingleton<IAppConfiguration, AppConfiguration>();
        collection.AddSingleton<ILoadingService>(serviceProvider =>
        {
            return new LoadingService(debounceMilliseconds: 500, minimumDisplayMilliseconds: 300);
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

