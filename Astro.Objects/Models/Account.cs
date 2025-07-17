using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Account
    {
        [JsonPropertyName("id")]
        public short Id { get; set; } = 0;
        [JsonPropertyName("accountNumber")]
        public string AccountNumber { get; set; } = string.Empty;
        [JsonPropertyName("accountName")]
        public string AccountName { get; set; } = string.Empty;
        [JsonPropertyName("accountType")]
        public short AccountType { get; set; } = 0;
        [JsonPropertyName("provider")]
        public short Provider { get; set;} = 0;

        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.Account);
    }

    public class AccountProvider
    {
        [JsonPropertyName("id")]
        public short Id { get; set; } = 0;
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("type")]
        public short Type { get; set; } = 0;
    }
}
