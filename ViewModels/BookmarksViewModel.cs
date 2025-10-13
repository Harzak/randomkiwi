using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace randomkiwi.ViewModels;

public sealed partial class BookmarksViewModel : BaseRoutableViewModel
{
    private readonly IBookmarksRepository _bookmarksRepository;

    public override string Name => nameof(BookmarksViewModel);

    /// <inheritdoc/>
    public override bool CanBeConfigured => true;
    
    [ObservableProperty]
    private List<Bookmark>? _bookmarks;

    public BookmarksViewModel(INavigationService navigationService, IBookmarksRepository bookmarksRepository) : base(navigationService)
    {
        _bookmarksRepository = bookmarksRepository ?? throw new ArgumentNullException(nameof(bookmarksRepository));
    }

    public override async Task OnInitializedAsync()
    {
        OperationResult<BookmarkList> result = await  _bookmarksRepository.LoadAsync().ConfigureAwait(false);
        if (result.IsSuccess && result.HasContent)
        {
            this.Bookmarks = result.Content.Articles.ToList();
        }
    }

    [RelayCommand]
    private void OpenBookmark(Bookmark bookmark)
    {
       throw new NotImplementedException();
    }
}