using System.Collections.ObjectModel;

namespace randomkiwi.Models;

/// <summary>
/// Represents a collection of bookmarked articles along with metadata
/// </summary>
public sealed class BookmarkList
{
    public DateTimeOffset LastWriteUtc { get; set; }
    public Collection<Bookmark> Articles { get; init; }

    public BookmarkList()
    {
        this.Articles = [];
    }
}

