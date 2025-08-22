using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models.Transactions
{
    public class Cost
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        [JsonPropertyName("value")]
        public int Value { get; set; } = 0;
    }
    public class CostCollection : Collection<Cost>
    {
        public int GetTotal()
        {
            var total = 0;
            foreach (var item in this)
            {
                total += item.Value;
            }
            return total;
        }
    }
}
