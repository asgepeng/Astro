using Astro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.ViewModels
{
    public class AccountViewModel
    {
        [JsonPropertyName("account")]
        public Account Account { get; set; } = new Account();
        [JsonPropertyName("providers")]
        public List<AccountProvider> Providers { get; set; } = new List<AccountProvider>();
    }
}
