namespace randomkiwi.Repositories;

/// <summary>
/// Provides functionality to manage and persist bookmarks, including loading, saving, and checking bookmark status.
/// </summary>
public sealed class BookmarksRepository : IBookmarksRepository
{
    private readonly IJsonStorage<BookmarkList> _jsonStorage;
    private BookmarkList? _cache;

    public BookmarksRepository(IJsonStorage<BookmarkList> jsonStorage)
    {
        _jsonStorage = jsonStorage;
    }

    /// <inheritdoc/>
    public async Task<OperationResult<BookmarkList>> LoadAsync()
    {
        OperationResult<BookmarkList> result = new();
        if (_cache != null)
        {
            return result.WithValue(_cache).WithSuccess();
        }

        result = await _jsonStorage.LoadAsync().ConfigureAwait(false);

        if (result.IsSuccess)
        {
            _cache = result.Content;
        }
        return result;
    }

    /// <inheritdoc/>
    public async Task<OperationResult> SaveAsync(Bookmark bookmarks)
    {
        ArgumentNullException.ThrowIfNull(bookmarks, nameof(bookmarks));
        OperationResult result = new();
        BookmarkList newList = new();

        if (_jsonStorage.Exists)
        {
            OperationResult<BookmarkList> loadResult = await _jsonStorage.LoadAsync().ConfigureAwait(false);

            if (loadResult.IsFailed || !loadResult.HasContent)
            {
                return loadResult;
            }
            if (loadResult.Content.Articles.Any(x => x.WikipediaIdentifier == bookmarks.WikipediaIdentifier))
            {
                return loadResult.WithSuccess();
            }
            newList = loadResult.Content;
        }

        newList.Articles.Add(bookmarks);
        newList.LastWriteUtc = DateTime.UtcNow;

        await _jsonStorage.SaveAsync(newList).ConfigureAwait(false);

        _cache = newList;
        return result.WithSuccess();
    }

    /// <inheritdoc/>
    public bool IsBookmarked(int wikipediaIdentifier)
    {
        if (_cache != null)
        {
            return _cache.Articles.Any(x => x.WikipediaIdentifier == wikipediaIdentifier);
        }
        return false;
    }
}