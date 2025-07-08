using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class QueryParameter
    {
        [JsonPropertyName("filters")] List<QueryFilter> Filters { get; set; } = new List<QueryFilter>();
        [JsonPropertyName("sort")] string Sort { get; set; } = string.Empty;
        [JsonPropertyName("page")] int Page { get; set; } = 1;
        [JsonPropertyName("pageSize")] int PageSize { get; set; } = 10;
        [JsonPropertyName("totalCount")] int TotalCount { get; set; } = 0;
        [JsonPropertyName("totalPages")] int TotalPages { get; set; } = 0;
        [JsonPropertyName("search")] string Search { get; set; } = string.Empty;
    }

    public class QueryFilter
    {
        [JsonPropertyName("column")] public string Column { get;set; } = string.Empty;
        [JsonPropertyName("value")] public string Value { get; set; } = string.Empty;
    }
}
