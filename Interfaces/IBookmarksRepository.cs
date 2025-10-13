using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Interfaces;

/// <summary>
/// Defines methods for managing and persist bookmarks, including loading, saving, and checking bookmark status.
/// </summary>
public interface IBookmarksRepository
{
    /// <summary>
    /// Asynchronously loads a list of bookmarks from storage.
    /// </summary>
    Task<OperationResult<BookmarkList>> LoadAsync();

    /// <summary>
    /// Saves the specified bookmark asynchronously to persistent storage.
    /// </summary>
    Task<OperationResult> SaveAsync(Bookmark bookmarks);

    /// <summary>
    /// Determines whether the specified Wikipedia article is bookmarked.
    /// </summary>
    bool IsBookmarked(int wikipediaIdentifier);
}