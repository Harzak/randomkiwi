using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace randomkiwi.Dto;

public sealed class QueryResultDto
{
    [JsonPropertyName("pages")]
    public Dictionary<string, PageDto>? Pages { get; init; }
}
