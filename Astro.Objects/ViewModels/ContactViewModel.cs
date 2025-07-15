using Astro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.ViewModels
{
    public class ContactViewModel
    {
        [JsonConstructor] public ContactViewModel() { }
        [JsonPropertyName("contact")]
        public Contact Contact { get; set; } = new Contact();
        [JsonPropertyName("addresses")]
        public List<Address> Addresses { get; } = new List<Address>();
    }
}
