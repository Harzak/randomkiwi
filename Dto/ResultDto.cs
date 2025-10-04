using System.Text.Json.Serialization;

namespace randomkiwi.Dto;

public sealed class ResultDto
{
    [JsonPropertyName("batchcomplete")]
    public string? BatchComplete { get; init; }

    [JsonPropertyName("query")]
    public QueryResultDto? Query { get; init; }
}

