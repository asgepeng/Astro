using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Data;
using System.Security.Claims;
using System.Data.Common;

using Astro.Data;
using Astro.Server.Memory;
using Astro.Server.Extensions;
using Astro.Extensions;

namespace Astro.Server.Middlewares
{
    public class TokenValidator
    {
        public Task ValidateAsync(MessageReceivedContext context)
        {
            string requestToken = context.Request.GetToken();
            string requestUserAgent = context.Request.Headers.UserAgent.ToString();
            var requestIPV4 = context.Request.GetIpAddress().ToInet();

            var data = TokenStore.Get(requestToken);
            if (data is null)
            {
                TokenStore.Delete(requestToken);

                context.Fail("Unauthorized request");
                return Task.CompletedTask;
            }


            using var stream = new MemoryStream(data);
            using var reader = new Astro.Binaries.BinaryDataReader(stream);

            var expiredDate = new DateTime(BitConverter.ToInt64(data, 0));
            if (expiredDate < DateTime.Now)
            {
                context.Fail("Expired token");
                return Task.CompletedTask;
            }
            var IPV4 = data.SubBytes(8, 4);
            if (!IPV4.SequenceEqual(requestIPV4))
            {
                context.Fail("IP Address or User Agent do not match");
                return Task.CompletedTask;
            }

            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Actor, BitConverter.ToInt16(data, 14).ToString()),
                new Claim(ClaimTypes.Role, BitConverter.ToInt16(data, 16).ToString())
            }, "Bearer");

            context.Principal = new ClaimsPrincipal(identity);
            context.Success();

            return Task.CompletedTask;
        }
    }
}
