using Astro.Models;
using Astro.Winform.Classes;
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
            using (var builder = new ListOptionBuilder(await HttpClientSingleton.GetStreamAsync("/data/regions/countries")))
            {
                return builder.GetListOption(typeof(short));
            }
        }
        internal static async Task<ListOption> GetStateOptionsAsync(short countryId)
        {
            using (var builder = new ListOptionBuilder(await HttpClientSingleton.GetStreamAsync("/data/regions/states/" + countryId.ToString())))
            {
                return builder.GetListOption(typeof(short));
            }
        }
        internal static async Task<ListOption> GetCityOptionsAsync(short stateId)
        {
            using (var builder = new ListOptionBuilder(await HttpClientSingleton.GetStreamAsync("/data/regions/cities/" + stateId.ToString())))
            {
                return builder.GetListOption(typeof(int));
            }
        }
        internal static async Task<ListOption> GetRoleOptionsAsync()
        {
            using (var builder = new ListOptionBuilder(await HttpClientSingleton.GetStreamAsync("/data/users/role-options")))
            {
                return builder.GetListOption(typeof(short));
            }
        }
    }
}
