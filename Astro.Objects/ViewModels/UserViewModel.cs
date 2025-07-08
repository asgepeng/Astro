using System.Text.Json;
using System.Text.Json.Serialization;
using Astro.Models;

namespace Astro.ViewModels
{
    public class UserViewModel
    {
        [JsonConstructor] public UserViewModel(User user) { User = user; }
        [JsonPropertyName("user")] public User User { get; set; }
        [JsonPropertyName("roles")] public ListOption Roles { get; set; } = new ListOption();
        [JsonPropertyName("countries")] public ListOption Countries { get; set; } = new ListOption();
        [JsonPropertyName("states")] public ListOption States { get; set; } = new ListOption();
        [JsonPropertyName("cities")] public ListOption Cities { get; set; } = new ListOption();
        [JsonPropertyName("addressInfo")] public AddressInfo AddressInfo { get; set; } = new AddressInfo();

        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.UserViewModel);
        public static UserViewModel? Create(string json) => JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.UserViewModel);
    }
}
