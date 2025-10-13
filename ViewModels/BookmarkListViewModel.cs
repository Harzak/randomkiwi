using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace randomkiwi.ViewModels;

public sealed partial class BookmarkListViewModel : BaseRoutableViewModel
{
    private readonly IBookmarksRepository _bookmarksRepository;
    private readonly IViewModelFactory _viewModelFactory;

    public override string Name => nameof(BookmarkListViewModel);

    /// <inheritdoc/>
    public override bool CanBeConfigured => true;

    [ObservableProperty]
    private List<Bookmark>? _bookmarks;

    public BookmarkListViewModel(
        INavigationService navigationService, 
        IBookmarksRepository bookmarksRepository, 
        IViewModelFactory viewModelFactory)
    : base(navigationService)
    {
        _bookmarksRepository = bookmarksRepository ?? throw new ArgumentNullException(nameof(bookmarksRepository));
        _viewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));
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
        IRoutableViewModel bookmarksViewModel = _viewModelFactory.CreateBookmarkViewModel(bookmark);
        await base.NavigationService.NavigateToAsync(bookmarksViewModel).ConfigureAwait(false);
    }
}