using Astro.Models;
using Astro.Winform.Classes;

namespace Astro.Winform.Helpers
{
    internal static class ListOptionHelper
    {
        internal static async Task<ListOption> GetCountryOptionsAsync()
        {
            var list = new ListOption();
            using (var stream = await WClient.GetStreamAsync("/data/regions/countries"))
            using (var reader = new IO.Reader(stream))
            {
                while (reader.Read())
                {
                    list.Add(new Option()
                    {
                        Id = reader.ReadInt16(),
                        Text = reader.ReadString()
                    });
                }
            }
            return list;
        }
        internal static async Task<ListOption> GetStateOptionsAsync(short countryId)
        {
            var list = new ListOption();
            using (var stream = await WClient.GetStreamAsync("/data/regions/states/" + countryId.ToString()))
            using (var reader = new IO.Reader(stream))
            {
                while (reader.Read())
                {
                    list.Add(new Option()
                    {
                        Id = reader.ReadInt16(),
                        Text = reader.ReadString()
                    });
                }
            }
            return list;
        }
        internal static async Task<ListOption> GetCityOptionsAsync(short stateId)
        {
            var list = new ListOption();
            using (var stream = await WClient.GetStreamAsync("/data/regions/cities/" + stateId.ToString()))
            using (var reader = new IO.Reader(stream))
            {
                while (reader.Read())
                {
                    list.Add(new Option()
                    {
                        Id = reader.ReadInt32(),
                        Text = reader.ReadString()
                    });
                }
            }
            return list;
        }
        internal static async Task<ListOption> GetRoleOptionsAsync()
        {
            var list = new ListOption();
            using (var stream = await WClient.GetStreamAsync("/data/users/role-options"))
            using (var reader = new IO.Reader(stream))
            {
                while (reader.Read())
                {
                    list.Add(new Option()
                    {
                        Id = reader.ReadInt16(),
                        Text = reader.ReadString()
                    });
                }
            }
            return list;
        }
    }
}
