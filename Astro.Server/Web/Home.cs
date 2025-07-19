using Astro.Server.Memory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Server.Web
{
    internal static class Home
    {
        internal static void MapHomeEndPoints(this WebApplication app)
        {
            app.MapGet("/", Index);
        }
        private static async Task<IResult> Index()
        {
            return await Task.FromResult(Results.Content(Layout.MainLayout.Replace("@content", TokenStore.ToView()), "text/html"));
        }

    }
}
