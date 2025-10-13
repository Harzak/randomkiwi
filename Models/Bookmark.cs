using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Models;

/// <summary>
/// Represents a bookmark that stores metadata about a specific Wikipedia entry.
/// </summary>
public sealed record Bookmark
{
    public required Guid Identifier { get; init; }
    public required int WikipediaIdentifier { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required Uri Url { get; init; }
    public required DateTimeOffset DateAddedUtc { get; init; }
}