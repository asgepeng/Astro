using System.Text.Json;
using System.Text.Json.Serialization;
using Astro.Models;

namespace Astro.ViewModel
{
    public class UserViewModel
    {
        [JsonPropertyName("user")] public User User { get; } = new User();
        [JsonPropertyName("roles")] public List<Role> Roles { get; } = new List<Role>();
        [JsonPropertyName("addresses")] public List<AddressInfo> Addresses { get; } = new List<AddressInfo>();
        [JsonPropertyName("emails")] public List<EmailInfo> Emails { get; } = new List<EmailInfo>();
        [JsonPropertyName("phones")] public List<PhoneInfo> Phones { get; } = new List<PhoneInfo>();

        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.UserViewModel);
    }
}
