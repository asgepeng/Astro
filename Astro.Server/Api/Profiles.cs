using Astro.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Server.Api
{
    internal static class Profiles
    {
        internal static void MapProfileEndPoints(this WebApplication app)
        {
            app.MapGet("/profile", GetProfileAsync).RequireAuthorization();
            app.MapPost("/profile", UpdateProfileAsync).RequireAuthorization();
        }
        internal static async Task<IResult> GetProfileAsync(IDatabase db, HttpContext context)
        {
            var commandText = """
                SELECT id, name
                """;
            return await Task.FromResult(Results.Ok());
        }
        internal static async Task<IResult> UpdateProfileAsync(IDatabase db, HttpContext context)
        {
            return await Task.FromResult(Results.Ok());
        }
    }
}
