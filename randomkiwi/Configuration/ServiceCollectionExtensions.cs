using Microsoft.Extensions.Logging;
using randomkiwi.Navigation.ViewModels;
using randomkiwi.Navigation.WebPage;
using randomkiwi.Services.Http;
using randomkiwi.Configuration.Models;
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
    public static void AddCoreServices(this IServiceCollection services)
    {
        services.AddConfiguration();
        services.AddHttpServices();
        services.AddView();
        services.AddViewModels();
        services.AddAppServices();
        services.AddRepositoryServices();
    }

    private static void AddConfiguration(this IServiceCollection services)
    {
        services.AddOptions<AppSettings>().BindConfiguration(AppSettings.SectionName);
        services.AddOptions<UserMetricsSettings>().BindConfiguration(UserMetricsSettings.SectionName);
        services.AddOptions<ArticleCatalogSettings>().BindConfiguration(ArticleCatalogSettings.SectionName);
        services.AddOptions<HttpClientSettings>().BindConfiguration(HttpClientSettings.SectionName);
        services.AddOptions<WikipediaSettings>().BindConfiguration(WikipediaSettings.SectionName);
        services.AddSingleton<IAppSettingsProvider, AppSettingsProvider>();
    }

    private static void AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<IArticleCatalog, WikipediaArticleCatalog>();
        services.AddSingleton<IWikipediaAPIClient, WikipediaAPIClient>();
        services.AddSingleton<IWikipediaUrlBuilder, WikipediaUrlBuilder>();
        services.AddSingleton<IUserMetricsService, UserMetricsService>();
        services.AddSingleton<IWebViewConfigurator, WebViewConfigurator>();
        services.AddTransient<IWebViewManager, WebViewManager>();
        services.AddSingleton<IScriptLoader, ScriptLoader>();
        services.AddSingleton<INavigationHandler<IRoutableViewModel>, ViewModelNavigationHandler>();
        services.AddSingleton<INavigationHandler<IRoutableItem>, WebPageNavigationHandler>();
        services.AddSingleton<IViewModelNavigationService, ViewModelNavigationService>();
        services.AddSingleton<IWebPageNavigationService, WebPageNavigationService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IAppConfiguration, AppConfiguration>();
        services.AddSingleton<ILoadingService>(serviceProvider =>
        {
            return new LoadingService(debounceMilliseconds: 500, minimumDisplayMilliseconds: 300);
        });
    }

    private static void AddRepositoryServices(this IServiceCollection services)
    {
        services.AddSingleton<IUserPreferenceRepository, UserPreferenceRepository>();
        services.AddSingleton<IBookmarksRepository, BookmarksRepository>();

        services.AddSingleton<IJsonStorage<UserPreferenceModel>>(provider =>
        {
            var settingsProvider = provider.GetRequiredService<IAppSettingsProvider>();
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appDirectory = Path.Combine(appDataPath, settingsProvider.App.Name);
            return new JsonStorage<UserPreferenceModel>(
                appDirectory,
                settingsProvider.App.UserPreferencesFile,
                provider.GetRequiredService<ILogger<JsonStorage<UserPreferenceModel>>>());
        });

        services.AddSingleton<IJsonStorage<BookmarkList>>(provider =>
        {
            var settingsProvider = provider.GetRequiredService<IAppSettingsProvider>();
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appDirectory = Path.Combine(appDataPath, settingsProvider.App.Name);
            return new JsonStorage<BookmarkList>(
                appDirectory,
                settingsProvider.App.BookmarksFile,
                provider.GetRequiredService<ILogger<JsonStorage<BookmarkList>>>());
        });
    }

    private static void AddView(this IServiceCollection services)
    {
        services.AddSingleton<MainView>();
    }

    private static void AddViewModels(this IServiceCollection services)
    {
        services.AddSingleton<IViewModelFactory, ViewModelFactory>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<RandomArticleViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<BookmarkListViewModel>();
        services.AddTransient<WikipediaWebViewViewModel>();
    }

    private static void AddHttpServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpClientOptionFactory, HttpClientOptionFactory>();

        // Register HTTP client with the name from configuration
        // We'll resolve the name during service registration using a factory
        services.AddHttpClient<IWikipediaAPIClient, WikipediaAPIClient>((serviceProvider, client) =>
        {
            var settingsProvider = serviceProvider.GetRequiredService<IAppSettingsProvider>();
            // The client can be configured here if needed
        })
        .ConfigurePrimaryHttpMessageHandler(CreatePlatformOptimizedHandler);

        // Also register a named HTTP client for backward compatibility
        services.AddHttpClient("randomwiki")
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

