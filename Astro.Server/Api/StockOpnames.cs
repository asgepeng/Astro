using Astro.Data;
using Astro.Models;
using Astro.Server.Binaries;
using Astro.Server.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Server.Api
{
    internal static class StockOpnames
    {
        internal static void MapStockOpnameEndPoints(this WebApplication app)
        {
            app.MapGet("/trans/stock-opnames", GetAllAsync).RequireAuthorization();
            app.MapGet("/trans/stock-opnames/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/trans/stock-opnames", CreateAsync).RequireAuthorization();
        }
        private static async Task<IResult> GetAllAsync(IDatabase db, HttpContext context)
        {
            if (context.Request.IsDesktopAppRequest()) return Results.File(await db.GetAccountDataTable(), "application/octet-stream");
            return Results.Ok();
        }
        private static async Task<IResult> GetByIdAsync(int id, IDatabase db, HttpContext context)
        {
            return await Task.FromResult(Results.Ok());
        }
        private static async Task<IResult> CreateAsync(StockOpname stockOpname, IDatabase db, HttpContext context)
        {
            return await Task.FromResult(Results.Ok());
        }
    }
}
