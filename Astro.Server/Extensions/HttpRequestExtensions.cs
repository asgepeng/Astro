using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Server.Extensions
{
    internal static class HttpRequestExtensions
    {
        internal static async Task<MemoryStream> GetMemoryStreamAsync(this HttpRequest request)
        {
            request.EnableBuffering();

            var memoryStream = new MemoryStream();
            await request.Body.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            request.Body.Position = 0;

            return memoryStream;    
        }
        internal static bool IsDesktopAppRequest(this HttpRequest request)
        {
            return request.Headers.UserAgent.ToString() == "astro.winform.app";     
        }
    }
}
