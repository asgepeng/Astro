﻿using Astro.Helpers;
using Astro.Data;
using Astro.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Data;


namespace Astro.Server.Api
{
    internal static class Regions
    {
        internal static void MapRegionEndPoints(this WebApplication app)
        {
            app.MapGet("data/regions/countries", GetCountriesAsync).RequireAuthorization();
            app.MapGet("data/regions/states/{id}", GetProvincesAsync).RequireAuthorization();
            app.MapGet("data/regions/cities/{id}", GetCitiesAsync).RequireAuthorization();
        }
        internal static async Task<IResult> GetCountriesAsync(IDatabase db, HttpContext contxt)
        {
            var isWinformApp = Application.IsWinformApp(contxt.Request);
            var commandText = """
                select country_id, country_name
                from countries
                order by country_name
                """;
            if (isWinformApp)
            {
                using (var builder = new IO.Writer())
                {
                    await db.ExecuteReaderAsync(async reader =>
                    {
                        while (await reader.ReadAsync())
                        {
                            builder.WriteInt16(reader.GetInt16(0));
                            builder.WriteString(reader.GetString(1));
                        }
                    }, commandText);
                    return Results.File(builder.ToArray(), "application/octet-stream");
                }
            }

            var list = new ListOption();
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    list.Add(new Option()
                    {
                        Id = (int)reader.GetInt16(0),
                        Text = reader.GetString(1)
                    });
                }
            }, commandText);
            return Results.Ok(list);
        }
        internal static async Task<IResult> GetProvincesAsync(short id, IDatabase db, HttpContext context)
        {
            var isWinformApp = Application.IsWinformApp(context.Request);
            var commandText = """
                select state_id, state_name
                from states
                where country_id = @id
                order by state_name;
                """;
            if (isWinformApp)
            {
                using (var builder = new IO.Writer())
                {
                    await db.ExecuteReaderAsync(async reader =>
                     {
                         while (await reader.ReadAsync())
                         {
                             builder.WriteInt16(reader.GetInt16(0));
                             builder.WriteString(reader.GetString(1));
                         }
                     }, commandText, db.CreateParameter("id", id, DbType.Int16));
                    return Results.File(builder.ToArray(), "application/octet-stream");
                }
            }

            var list = new ListOption();            
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    list.Add(new Option()
                    {
                        Id = (int)reader.GetInt16(0),
                        Text = reader.GetString(1)
                    });
                }
            }, commandText, db.CreateParameter("id", id, DbType.Int16));
            return Results.Ok(list);
        }
        internal static async Task<IResult> GetCitiesAsync(short id, IDatabase db, HttpContext context)
        {
            var commandText = """
                select city_id, city_name
                from cities
                where state_id = @id
                order by city_name;
                """;
            var isWinformApp = Application.IsWinformApp(context.Request);
            if (isWinformApp)
            {
                using (var builder = new IO.Writer())
                {
                    await db.ExecuteReaderAsync(async reader =>
                    {
                        while (await reader.ReadAsync())
                        {
                            builder.WriteInt32(reader.GetInt32(0));
                            builder.WriteString(reader.GetString(1));
                        }
                    }, commandText, db.CreateParameter("id", id, DbType.Int16));
                    return Results.File(builder.ToArray(), "application/octet-stream");
                }
            }

            var list = new ListOption();
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    list.Add(new Option()
                    {
                        Id = (int)reader.GetInt16(0),
                        Text = reader.GetString(1)
                    });
                }
            }, commandText, db.CreateParameter("id", id, DbType.Int16));
            return Results.Ok(list);
        }
    }
}
