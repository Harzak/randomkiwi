using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace randomkiwi.Models;

/// <summary>
/// Represents metadata about a Wikipedia article
/// </summary>
public sealed record WikipediaArticleMetadata
{
    public required int Id { get;  init; }
    public required string  Title { get;  init; }
    public required string Description { get; init; }
    public required Uri Url { get;  init; }
    public required int  Namespace { get;  init; }
}
