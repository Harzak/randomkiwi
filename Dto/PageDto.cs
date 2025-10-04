using System.Text.Json.Serialization;

namespace randomkiwi.Dto;

public record PageDto
{
    [JsonPropertyName("pageid")]
    public int PageId { get; init; }

    [JsonPropertyName("ns")]
    public int Namespace { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("pageprops")]
    public PagePropertyDto? PageProps { get; init; }
}