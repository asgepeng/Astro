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
        [JsonPropertyName("country")]
        public Country Country { get; set; } = new Country();
        [JsonPropertyName("type")] 
        public short Type { get; set; } = 0;
        [JsonPropertyName("zipCode")] 
        public string ZipCode { get; set; } = "";
        [JsonPropertyName("isPrimary")] 
        public bool IsPrimary { get; set; } = false;

        public Address Clone()
        {
            return new Address()
            {
                Type = this.Type,
                Id = this.Id,
                StreetAddress = this.StreetAddress,
                City = new City()
                {
                    Id = this.City.Id,
                    Name = this.City.Name
                },
                StateOrProvince = new Province()
                {
                    Id = this.StateOrProvince.Id,
                    Name = this.StateOrProvince.Name
                },
                Country = new Country()
                {
                    Id = this.Country.Id,
                    Name = this.Country.Name
                },
                ZipCode = this.ZipCode,
                IsPrimary = this.IsPrimary
            };
        }
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

        public Phone Clone()
        {
            return new Phone()
            {
                Id = this.Id,
                Number = this.Number,
                Type = this.Type,
                IsPrimary = this.IsPrimary
            };
        }
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

        public Email Clone()
        {
            return new Email()
            {
                Id = this.Id,
                Address = this.Address,
                Type = this.Type,
                IsPrimary = this.IsPrimary
            };
        }
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
