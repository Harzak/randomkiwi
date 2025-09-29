using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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