using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

