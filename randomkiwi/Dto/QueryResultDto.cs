using System.Text.Json.Serialization;

namespace randomkiwi.Dto;

public sealed class QueryResultDto
{
    [JsonPropertyName("pages")]
    public Dictionary<string, PageDto>? Pages { get; init; }
}
