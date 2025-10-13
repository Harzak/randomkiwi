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
    private readonly IArticleCatalog _articleCatalog;
    private readonly ILoadingService _loadingService;
    private readonly IBookmarksRepository _bookmarksRepository;
    private readonly IAppConfiguration _appConfiguration;
    private readonly INavigationService _navigationService;
    private readonly WikipediaWebViewViewModel _webViewViewModel;

    public ViewModelFactory(
        IBookmarksRepository bookmarksRepository,
        ILoadingService loadingService,
        INavigationService navigationService,
        IArticleCatalog articleCatalog,
        IAppConfiguration appConfiguration,
        WikipediaWebViewViewModel webViewViewModel)
    {
        _bookmarksRepository = bookmarksRepository ?? throw new ArgumentNullException(nameof(bookmarksRepository));
        _loadingService = loadingService ?? throw new ArgumentNullException(nameof(loadingService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _webViewViewModel = webViewViewModel ?? throw new ArgumentNullException(nameof(webViewViewModel));
        _articleCatalog = articleCatalog ?? throw new ArgumentNullException(nameof(articleCatalog));
        _appConfiguration = appConfiguration ?? throw new ArgumentNullException(nameof(appConfiguration));
    }

    public IRoutableViewModel CreateBookmarkViewModel(Bookmark bookmark)
    {
        return new BookmarkViewModel(bookmark, _webViewViewModel, _navigationService, _loadingService);
    }

    public IRoutableViewModel CreateRandomArticleViewModel()
    {
        return new RandomArticleViewModel(_webViewViewModel, _articleCatalog, _loadingService, _navigationService, _bookmarksRepository, _appConfiguration);
    }

    public IRoutableViewModel CreateBookmarkListViewModel()
    {
        return new BookmarkListViewModel(_navigationService, _bookmarksRepository, this);
    }

    public IRoutableViewModel CreateSettingsViewModel()
    {
        return new SettingsViewModel(_appConfiguration, _navigationService);
    }
}
