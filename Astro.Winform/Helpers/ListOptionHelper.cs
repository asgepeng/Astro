using Astro.Models;
using Astro.Winform.Classes;
using Astro.Utils;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Quic;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Winform.Helpers
{
    internal static class ListOptionHelper
    {
        internal static async Task<ListOption> GetCountryOptionsAsync()
        {
            using (var stream = await HttpClientSingleton.GetStreamAsync("/data/regions/countries"))
            using (var builder = new ListOptionBuilder(stream))
            {
                return builder.ToListOption(typeof(short));
            }
        }
        internal static async Task<ListOption> GetStateOptionsAsync(short countryId)
        {
            using (var stream = await HttpClientSingleton.GetStreamAsync("/data/regions/states/" + countryId.ToString()))
            using (var builder = new ListOptionBuilder(stream))
            {
                var type = typeof(short);
                return builder.ToListOption(type);
            }
        }
        internal static async Task<ListOption> GetCityOptionsAsync(short stateId)
        {
            using (var stream = await HttpClientSingleton.GetStreamAsync("/data/regions/cities/" + stateId.ToString()))
            using (var builder = new ListOptionBuilder(stream))
            {
                return builder.ToListOption(typeof(int));
            }
        }
        internal static async Task<ListOption> GetRoleOptionsAsync()
        {
            using (var stream = await HttpClientSingleton.GetStreamAsync("/data/users/role-options"))
            using (var builder = new ListOptionBuilder(stream))
            {
                return builder.ToListOption(typeof(short));
            }
        }
    }
}
