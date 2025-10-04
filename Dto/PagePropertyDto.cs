using System.Text.Json.Serialization;

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
