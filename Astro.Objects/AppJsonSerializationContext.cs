using Astro.Models;
using Astro.ViewModel;
using System.Text.Json.Serialization;

[JsonSerializable(typeof(ProductViewModel))]
[JsonSerializable(typeof(ContactViewModel))]
[JsonSerializable(typeof(RoleViewModel))]
[JsonSerializable(typeof(UserViewModel))]

[JsonSerializable(typeof(List<Unit>))]
[JsonSerializable(typeof(List<Category>))]
[JsonSerializable(typeof(List<Permission>))]
[JsonSerializable(typeof(List<Country>))]
[JsonSerializable(typeof(List<Province>))]
[JsonSerializable(typeof(List<City>))]
[JsonSerializable(typeof(List<Menu>))]

[JsonSerializable(typeof(ListSection))]
[JsonSerializable(typeof(Menu))]
[JsonSerializable(typeof(Section))]
[JsonSerializable(typeof(UserInfo))]
[JsonSerializable(typeof(Unit))]
[JsonSerializable(typeof(Product))]
[JsonSerializable(typeof(Category))]
[JsonSerializable(typeof(City))]
[JsonSerializable(typeof(Province))]
[JsonSerializable(typeof(Country))]
[JsonSerializable(typeof(LoginResponse))]
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(Permission))]
[JsonSerializable(typeof(Role))]
[JsonSerializable(typeof(User))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}