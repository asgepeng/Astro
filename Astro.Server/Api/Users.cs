using Astro.Data;
using Astro.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Server.Api
{
    internal static class Users
    {
        internal static void MapUserEndPoints(this WebApplication app)
        {
            app.MapGet("/data/users", GetAllAsync).RequireAuthorization();
            app.MapGet("/data/users/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/data/users", CreateAsync).RequireAuthorization();
            app.MapPut("/data/users", UpdateAsync).RequireAuthorization();
            app.MapDelete("/data/users/{id}", DeleteAsync).RequireAuthorization();
        }
        internal static async Task<IResult> GetAllAsync(IDatabase db)
        {
            return await Task.FromResult(Results.Ok());
        }
        internal static async Task<IResult> GetByIdAsync(int id, IDatabase db)
        {
            return await Task.FromResult(Results.Ok());
        }
        internal static async Task<IResult> CreateAsync(User user, IDatabase db, HttpContext context)
        {
            return await Task.FromResult(Results.Ok());
        }
        internal static async Task<IResult> UpdateAsync(User user, IDatabase db, HttpContext context)
        {
            return await Task.FromResult(Results.Ok());
        }
        internal static async Task<IResult> DeleteAsync(int id, IDatabase db, HttpContext context)
        {
            return await Task.FromResult(Results.Ok());
        }
    }
}
