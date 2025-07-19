using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Server.Web
{
    internal static class Layout
    {
        internal static string MainLayout
        {
            get
            {
                return File.ReadAllText(AppContext.BaseDirectory + "wwwroot\\MainLayout.html");
            }
        }
    }
}
