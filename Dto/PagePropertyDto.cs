using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace randomkiwi.Dto;

public sealed record PagePropertyDto
{
    [JsonPropertyName("displaytitle")]
    public string? DisplayTitle { get; init; }

    [JsonPropertyName("wikibase_item")]
    public string? WikibaseItem { get; init; }

    [JsonPropertyName("defaultsort")]
    public string? DefaultSort { get; init; }
}
