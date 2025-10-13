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
    public static void AddCoreServices(this IServiceCollection services)
    {
        services.AddHttpServices();
        services.AddView();
        services.AddViewModels();
        services.AddAppServices();
        services.AddRepositoryServices();
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
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string appDirectory = Path.Combine(appDataPath, AppConsts.APP_NAME);
        services.AddSingleton<IJsonStorage<UserPreferenceModel>>(provider =>
        {
            return new JsonStorage<UserPreferenceModel>(
                appDirectory, 
                AppConsts.USER_PREFERENCES_FILE,
                provider.GetRequiredService<ILogger<JsonStorage<UserPreferenceModel>>>());
        });
        services.AddSingleton<IJsonStorage<BookmarkList>>(provider =>
        {
            return new JsonStorage<BookmarkList>(
                appDirectory, 
                AppConsts.BOOKMARKS_FILE,
                provider.GetRequiredService<ILogger<JsonStorage<BookmarkList>>>());
        });
        services.AddSingleton<IUserPreferenceRepository, UserPreferenceRepository>();
        services.AddSingleton<IBookmarksRepository, BookmarksRepository>();
    }

    private static void AddView(this IServiceCollection services)
    {
        services.AddSingleton<MainView>();
    }

    private static void AddViewModels(this IServiceCollection services)
    {
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<RandomArticleViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<BookmarkListViewModel>();
        services.AddTransient<WikipediaWebViewViewModel>();
    }

    private static void AddHttpServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpClientOptionFactory, HttpClientOptionFactory>();
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

