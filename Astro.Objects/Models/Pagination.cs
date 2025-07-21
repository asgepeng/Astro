using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Pagination
    {
        [JsonPropertyName("page")]
        public int Page { get; set; } = 1;
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; } = 20;
        [JsonPropertyName("search")]
        public string Search { get; set; } = string.Empty;
        [JsonPropertyName("orderBy")]
        public int OrderBy { get; set; } = 0;
        [JsonPropertyName("sortOrder")]
        public int SortOrder { get; set; } = 0;
        public int GetOffset()
        {
            return (Page - 1) * PageSize;
        }
        public string ToQuery()
        {
            return " ORDER BY " + OrderBy + (SortOrder == 0 ? " ASC " : " DESC ") +  
                "LIMIT " + PageSize.ToString() + " OFFSET " + (Page - 1).ToString();
        }
    }
}
