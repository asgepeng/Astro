using Astro.Data;
using Astro.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Data;
using DocumentFormat.OpenXml.InkML;
using Astro.Server.Extensions;


namespace Astro.Server.Api
{
    internal static class Regions
    {
        internal static void MapRegionEndPoints(this WebApplication app)
        {
            app.MapGet("data/regions/countries", GetCountriesAsync).RequireAuthorization();
            app.MapGet("data/regions/states/{id}", GetStatesAsync).RequireAuthorization();
            app.MapGet("data/regions/cities/{id}", GetCitiesAsync).RequireAuthorization();
            app.MapGet("data/regions/districts/{id}", GetDistrictsAscync).RequireAuthorization();
            app.MapGet("data/regions/villages/{id}", GetVillagesAsync).RequireAuthorization();
        }
        internal static async Task<IResult> GetCountriesAsync(IDBClient db, HttpContext context)
        {
            var commandText = """
                select countryid, name
                from countries
                order by name
                """;
            using (var builder = new Astro.Binaries.BinaryDataWriter())
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
        internal static async Task<IResult> GetStatesAsync(short id, IDBClient db, HttpContext context)
        {
            using (var builder = new Astro.Binaries.BinaryDataWriter())
            {
                var commandText = """
                    select stateid, name
                    from states
                    where countryid = @id
                    order by name;
                    """;
                var iPos = builder.ReserveInt32();
                var iCount = 0;
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        builder.WriteInt16(reader.GetInt16(0));
                        builder.WriteString(reader.GetString(1));
                        iCount++;
                    }
                }, commandText, db.CreateParameter("id", id, DbType.Int16));
                builder.WriteInt32(iCount, iPos);
                return Results.File(builder.ToArray(), "application/octet-stream");
            }
        }
        internal static async Task<IResult> GetCitiesAsync(short id, IDBClient db, HttpContext context)
        {
            using (var builder = new Astro.Binaries.BinaryDataWriter())
            {
                var commandText = """
                    select cityid, name
                    from cities
                    where stateid = @id
                    order by name;
                    """;
                var iPos = builder.ReserveInt32();
                var iCount = 0;
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        builder.WriteInt32(reader.GetInt32(0));
                        builder.WriteString(reader.GetString(1));
                        iCount++;
                    }
                }, commandText, db.CreateParameter("id", id, DbType.Int16));
                builder.WriteInt32(iCount, iPos);
                return Results.File(builder.ToArray(), "application/octet-stream");
            }
        }
        private static async Task<IResult> GetDistrictsAscync(int id, IDBClient db)
        {
            using (var writer = new Astro.Binaries.BinaryDataWriter())
            {
                var commandText = """
                SELECT districtid, name
                FROM districts
                WHERE cityid = @id
                ORDER BY name
                """;
                var iPos = writer.ReserveInt32();
                var iCount = 0;
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt32(reader.GetInt32(0));
                        writer.WriteString(reader.GetString(1));
                        iCount++;
                    }
                }, commandText, db.CreateParameter("id", id, DbType.Int32));
                writer.WriteInt32(iCount, iPos);
                return Results.File(writer.ToArray(), "application/octet-stream");
            }
        }
        private static async Task<IResult> GetVillagesAsync(int id, IDBClient db)
        {
            using (var writer = new Astro.Binaries.BinaryDataWriter())
            {
                var commandText = """
                    SELECT villageid, name
                    FROM villages
                    WHERE districtid = @id
                    """;
                var iPos = writer.ReserveInt32();
                var iCount = 0;
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt64(reader.GetInt64(0));
                        writer.WriteString(reader.GetString(1));
                        iCount++;
                    }
                }, commandText, db.CreateParameter("id", id, DbType.Int32));
                writer.WriteInt32(iCount, iPos);
                return Results.File(writer.ToArray(), "application/octet-stream");
            }
        }
    }
}
