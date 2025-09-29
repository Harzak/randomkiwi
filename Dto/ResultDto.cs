using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace randomkiwi.Dto;

public sealed class ResultDto
{
    [JsonPropertyName("batchcomplete")]
    public string? BatchComplete { get; init; }

    [JsonPropertyName("query")]
    public QueryResultDto? Query { get; init; }
}

