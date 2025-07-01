using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Astro;
using Astro.Models;
using Astro.ViewModel;

namespace Astro.ViewModel
{
    public class ProductViewModel
    {
        [JsonPropertyName("product")]
        public Product Product { get; set; }

        [JsonPropertyName("categories")]
        public List<Category> Categories { get; } = new List<Category>();
        [JsonPropertyName("units")]
        public List<Unit> Units { get; } = new List<Unit>();
    }
}
