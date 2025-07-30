using Astro.Data;
using Astro.Extensions;
using Astro.Models;
using Astro.Server.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Astro.Server.Api
{
    internal static class Documents
    {
        internal static void MapDocumentEndPoints(this WebApplication app)
        {
            app.MapPost("/documents/upload", UploadImageAsync).RequireAuthorization(); ;
            app.MapGet("/documents/download/{fileName}", DownloadAsync).RequireAuthorization();
        }
        private static async Task<IResult> UploadImageAsync(IDatabase db, HttpContext context)
        {
            var fileName = Guid.NewGuid().ToByteArray().ToHexString();
            using (var stream = await context.Request.GetMemoryStreamAsync())
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await stream.CopyToAsync(fileStream);
                }
            }
            return Results.Content(fileName, "text/plain");
        }
        public static IResult DownloadAsync(string fileName, HttpContext context)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "wwwroot", "images", fileName);
            if (fileName is null || !File.Exists(path))
            {
                return Results.NotFound();
            }
            return Results.File(File.ReadAllBytes(path), "application/octet-stream");
        }
    }
}
