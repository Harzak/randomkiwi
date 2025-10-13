using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace randomkiwi.ViewModels;

public sealed partial class BookmarkListViewModel : BaseRoutableViewModel
{
    private readonly IBookmarksRepository _bookmarksRepository;
    private readonly ILoadingService _loadingService;
    private readonly WikipediaWebViewViewModel _webViewViewModel;

    public override string Name => nameof(BookmarkListViewModel);

    /// <inheritdoc/>
    public override bool CanBeConfigured => true;

    [ObservableProperty]
    private List<Bookmark>? _bookmarks;

    public BookmarkListViewModel(
        INavigationService navigationService,
        ILoadingService loadingService,
        IBookmarksRepository bookmarksRepository,

        WikipediaWebViewViewModel webViewViewModel)
    : base(navigationService)
    {
        _bookmarksRepository = bookmarksRepository ?? throw new ArgumentNullException(nameof(bookmarksRepository));
        _loadingService = loadingService ?? throw new ArgumentNullException(nameof(loadingService));
        _webViewViewModel = webViewViewModel ?? throw new ArgumentNullException(nameof(webViewViewModel));
    }

    public override async Task OnInitializedAsync()
    {
        OperationResult<BookmarkList> result = await _bookmarksRepository.LoadAsync().ConfigureAwait(false);
        if (result.IsSuccess && result.HasContent)
        {
            this.Bookmarks = result.Content.Articles.OrderByDescending(x => x.DateAddedUtc).ToList();
        }
    }

    [RelayCommand]
    private async Task OpenBookmark(Bookmark bookmark)
    {
#pragma warning disable CA2000 // Dispose objects before losing scope
        IRoutableViewModel bookmarksViewModel = new BookmarkViewModel(bookmark, _webViewViewModel, base.NavigationService, _loadingService);
#pragma warning restore CA2000 // Dispose objects before losing scope
        await base.NavigationService.NavigateToAsync(bookmarksViewModel).ConfigureAwait(false);
    }
}