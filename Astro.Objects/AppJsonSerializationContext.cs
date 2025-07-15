using Astro.Models;
using Astro.ViewModels;
using System.Text.Json.Serialization;

[JsonSerializable(typeof(ProductViewModel))]
[JsonSerializable(typeof(ContactViewModel))]
[JsonSerializable(typeof(RoleViewModel))]
[JsonSerializable(typeof(UserViewModel))]

[JsonSerializable(typeof(List<Phone>))]
[JsonSerializable(typeof(List<Email>))]
[JsonSerializable(typeof(List<Address>))]
[JsonSerializable(typeof(List<Unit>))]
[JsonSerializable(typeof(List<Category>))]
[JsonSerializable(typeof(List<Permission>))]
[JsonSerializable(typeof(List<Country>))]
[JsonSerializable(typeof(List<Province>))]
[JsonSerializable(typeof(List<City>))]

[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(Address))]
[JsonSerializable(typeof(ListOption))]
[JsonSerializable(typeof(Option))]
[JsonSerializable(typeof(ChangePasswordRequest))]
[JsonSerializable(typeof(ListSection))]
[JsonSerializable(typeof(ListMenu))]
[JsonSerializable(typeof(Menu))]
[JsonSerializable(typeof(Section))]
[JsonSerializable(typeof(UserInfo))]
[JsonSerializable(typeof(Unit))]
[JsonSerializable(typeof(Product))]
[JsonSerializable(typeof(Category))]
[JsonSerializable(typeof(City))]
[JsonSerializable(typeof(Province))]
[JsonSerializable(typeof(Country))]
[JsonSerializable(typeof(AuthResponse))]
[JsonSerializable(typeof(Credential))]
[JsonSerializable(typeof(Permission))]
[JsonSerializable(typeof(Role))]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(CommonResult))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}