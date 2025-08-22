using Astro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    internal static class Application
    {
        internal static string ApiUrl { get; set; } = "";
        internal static string ApiToken { get; set; } = "";
        internal static UserInfo? User { get; set; } = null;
        internal static List<Branch> AccessableBranches { get; } = new List<Branch>();
        internal static Branch? Current { get; set; }
    }
}
