using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Address
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } = 0;
        [JsonPropertyName("streetAddress")]
        public string StreetAddress { get; set; } = "";
        [JsonPropertyName("city")]
        public City City { get; set; } = new City();
        [JsonPropertyName("state")]
        public Province StateOrProvince { get; set; } = new Province();
        [JsonPropertyName("type")] 
        public short Type { get; set; } = 0;
        [JsonPropertyName("zipCode")] 
        public string ZipCode { get; set; } = "";
        [JsonPropertyName("isPrimary")] 
        public bool IsPrimary { get; set; } = false;
    }

    public class Phone
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } = 0;
        [JsonPropertyName("number")] 
        public string Number { get; set; } = "";
        [JsonPropertyName("type")] 
        public short Type { get; set; } = 0;
        [JsonPropertyName("isPrimary")]
        public bool IsPrimary { get; set; } = false;
    }

    public class Email
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } = 0;
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;
        [JsonPropertyName("type")] 
        public short Type { get; set; } = 0;
        [JsonPropertyName("isPrimary")] 
        public bool IsPrimary { get; set; } = false;
    }
    public class AddressInfo
    {
        [JsonPropertyName("street")] 
        public string Street { get; set; } = string.Empty;
        [JsonPropertyName("city")] 
        public int City { get; set; } = 0;
        [JsonPropertyName("state")] 
        public short State { get; set; } = 0;
        [JsonPropertyName("country")] 
        public short Country { get; set; } = 0;
    }
}
