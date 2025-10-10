using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace randomkiwi.ViewModels;

public sealed partial class BookmarksViewModel : BaseRoutableViewModel
{
    public override string Name => nameof(BookmarksViewModel);

    /// <inheritdoc/>
    public override bool CanBeConfigured => true;
    
    [ObservableProperty]
    private List<BookmarkModel> _bookmarks;

    public BookmarksViewModel(INavigationService navigationService) : base(navigationService)
    {
        _bookmarks =
        [
            new BookmarkModel()
            {
                Identifier = Guid.NewGuid(),
                Name = "Example Bookmark 1",
                Description = "This is an example bookmark.",
                Url = new Uri("https://example.com/1"),
                DateAdded = DateTimeOffset.Now.AddDays(-10)
            },
            new BookmarkModel()
            {
                Identifier = Guid.NewGuid(),
                Name = "Example Bookmark 2",
                Description = "This is another example bookmark.",
                Url = new Uri("https://example.com/2"),
                DateAdded = DateTimeOffset.Now.AddDays(-5)
            },
            new BookmarkModel()
            {
                Identifier = Guid.NewGuid(),
                Name = "Example Bookmark 3",
                Description = "Yet another example bookmark.",
                Url = new Uri("https://example.com/3"),
                DateAdded = DateTimeOffset.Now.AddDays(-1)
            },
            new BookmarkModel()
            {
                Identifier = Guid.NewGuid(),
                Name = "Example Bookmark 4",
                Description = "More example bookmarks.",
                Url = new Uri("https://example.com/4"),
                DateAdded = DateTimeOffset.Now
            },
            new BookmarkModel()
            {
                Identifier = Guid.NewGuid(),
                Name = "Example Bookmark 5",
                Description = "The last example bookmark.",
                Url = new Uri("https://example.com/5"),
                DateAdded = DateTimeOffset.Now.AddHours(-12)
            },
            new BookmarkModel()
            {
                Identifier = Guid.NewGuid(),
                Name = "Example Bookmark 6",
                Description = "An additional example bookmark.",
                Url = new Uri("https://example.com/6"),
                DateAdded = DateTimeOffset.Now.AddHours(-6)
            }
        ];
    }

    [RelayCommand]
    private void OpenBookmark(BookmarkModel bookmark)
    {

    }

    [RelayCommand]
    private void DeleteBookmark(BookmarkModel bookmark)
    {

    }
}

public sealed record BookmarkModel
{
    public required Guid Identifier { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required Uri Url { get; init; }
    public required DateTimeOffset DateAdded { get; init; }
}