using Astro.Data;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Server.Websites
{
    public class HtmlLayout
    {
        public HtmlLayout(string filePath)
        {
            var html = File.ReadAllText(filePath);
            var headBeginIndex = html.IndexOf("<head>");
            if (headBeginIndex >= 0)
            {
                HtmlBegin = html.Substring(0, headBeginIndex);
            }
            else { HtmlBegin = ""; }
            headBeginIndex += 6;
            var headEndIndexIndex = html.IndexOf("</head>");
            if (headEndIndexIndex >= 0)
            {
                Head = html.Substring(headBeginIndex, headEndIndexIndex);
            }
            else { Head = ""; }
            var bodyBeginIndex = html.IndexOf("<body>");
            if (bodyBeginIndex >= 0)
            {
                bodyBeginIndex += 6;
                var bodyEndIndex = html.IndexOf("</body>");
                if (bodyEndIndex >= bodyBeginIndex)
                {
                    Body = html.Substring(bodyBeginIndex, bodyEndIndex - bodyBeginIndex);
                }
                else { Body = ""; }
            }
            else { Body = ""; }
        }
        public string HtmlBegin { get; }
        public string Head { get; }
        public string Body { get; }
    }
    internal class PageBuilder
    {
        private readonly StringBuilder sb;
        private readonly HtmlLayout _layout;
        internal PageBuilder(HtmlLayout layout)
        {
            sb = new StringBuilder();
            _layout = layout;
        }
        internal string Render()
        {
            sb.Append(_layout.HtmlBegin);
            sb.Append("<head>").Append(_layout.Head).Append("</head>");
            sb.Append("<body>").Append(_layout.Body).Append("</body>");
            sb.Append("</html>");
            return sb.ToString();
        }
    }
    internal static class WebLayout
    {
        static WebLayout()
        {
            _layouts = new HtmlLayout[1];
            var fileName = Path.Combine(AppContext.BaseDirectory, "layout.html");
            if (File.Exists(fileName))
            {
                _layouts[0] = new HtmlLayout(fileName);
            }
        }
        public static HtmlLayout GetLayout(int index)
        {
            return _layouts[index];
        }
        static readonly HtmlLayout[] _layouts;
    }
    internal static class Homepages
    {
        internal static void MapHomeEndPoints(this WebApplication app)
        {
            app.MapGet("/", Index);
        }
        private static IResult Index(IDBClient db, HttpContext context)
        {
            var builder = new PageBuilder(WebLayout.GetLayout(0));
            return Results.Content(builder.Render(), "text/html");
        }
    }
}
