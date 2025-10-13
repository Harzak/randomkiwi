using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Utilities;

public interface IViewModelFactory
{
    IRoutableViewModel CreateBookmarkViewModel(Bookmark bookmark);
    IRoutableViewModel CreateRandomArticleViewModel();
    IRoutableViewModel CreateBookmarkListViewModel();
    IRoutableViewModel CreateSettingsViewModel();
}

public sealed class ViewModelFactory : IViewModelFactory
{
    private readonly ILoadingService _loadingService;
    private readonly INavigationService _navigationService;
    private readonly IServiceProvider _serviceProvider;
    private readonly WikipediaWebViewViewModel _webViewViewModel;

    public ViewModelFactory(
        IServiceProvider serviceProvider,
        IBookmarksRepository bookmarksRepository,
        ILoadingService loadingService,
        INavigationService navigationService,
        IArticleCatalog articleCatalog,
        IAppConfiguration appConfiguration,
        WikipediaWebViewViewModel webViewViewModel)
    {
        _loadingService = loadingService ?? throw new ArgumentNullException(nameof(loadingService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _webViewViewModel = webViewViewModel ?? throw new ArgumentNullException(nameof(webViewViewModel));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public IRoutableViewModel CreateBookmarkViewModel(Bookmark bookmark)
    {
        return new BookmarkViewModel(bookmark, _webViewViewModel, _navigationService, _loadingService);
    }

    public IRoutableViewModel CreateRandomArticleViewModel()
    {
        return _serviceProvider.GetRequiredService<RandomArticleViewModel>();
    }

    public IRoutableViewModel CreateBookmarkListViewModel()
    {
        return _serviceProvider.GetRequiredService<BookmarkListViewModel>();
    }

    public IRoutableViewModel CreateSettingsViewModel()
    {
        return _serviceProvider.GetRequiredService<SettingsViewModel>();
    }
}
