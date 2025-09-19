using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Astro;
using Astro.Models;
using Astro.ViewModels;

namespace Astro.ViewModels
{
    public class ProductViewModel
    {
        [JsonPropertyName("product")]
        public Product Product { get; set; }

        [JsonPropertyName("categories")]
        public List<Option<short>> Categories { get; } = new List<Option<short>>();
        [JsonPropertyName("units")]
        public List<Option<short>> Units { get; } = new List<Option<short>>();
    }
}
