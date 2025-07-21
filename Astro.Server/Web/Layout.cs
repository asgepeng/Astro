using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Server.Web
{
    internal static class Layout
    {
        private static string _mainLayout = string.Empty;
        private static string _headerLayout = string.Empty;
        private static string _mainContent = string.Empty;
        private static string _script = string.Empty;
        internal static string MainLayout
        {
            get
            {
                if (string.IsNullOrEmpty(_mainLayout))
                {
                    _mainLayout = File.ReadAllText(AppContext.BaseDirectory + "wwwroot\\MainLayout.html");
                }
                return _mainLayout;
            }
        }
        internal static string CreateHtmlPage(string? stye = null, string? content = null, string? script = null)
        {
            return string.Join("", _headerLayout, stye, "</head>", content,  script, "</body>");
        }

    }
}
