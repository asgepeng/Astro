using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Data;
using System.Security.Claims;
using System.Data.Common;

using Astro.Data;
using Astro.Helpers;
using Astro.Server.Memory;

namespace Astro.Server.Middlewares
{
    public class TokenValidator
    {
        public Task ValidateAsync(MessageReceivedContext context)
        {
            string requestToken = Application.GetToken(context.Request);
            string requestUserAgent = context.Request.Headers.UserAgent.ToString();
            string requestIpAddress = Application.GetIpAddress(context.Request);

            var LoginInfo = TokenStore.Get(requestToken);
            if (LoginInfo is null)
            {
                TokenStore.Delete(requestToken);

                context.Fail("Unauthorized request");
                return Task.CompletedTask;
            }

            using var stream = new MemoryStream(LoginInfo);
            using var reader = new IO.Reader(stream);

            var expiredDate = reader.ReadDateTime();
            if (expiredDate < DateTime.Now)
            {
                context.Fail("Expired token");
                return Task.CompletedTask;
            }

            if (!string.Equals(requestIpAddress, reader.ReadString(), StringComparison.Ordinal) ||
                !string.Equals(requestUserAgent, reader.ReadString(), StringComparison.Ordinal))
            {
                context.Fail("IP Address or User Agent do not match");
                return Task.CompletedTask;
            }

            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, reader.ReadString()),
                new Claim(ClaimTypes.Actor, reader.ReadInt16().ToString()),
                new Claim(ClaimTypes.Role, reader.ReadInt16().ToString())
            }, "Bearer");

            context.Principal = new ClaimsPrincipal(identity);
            context.Success();

            return Task.CompletedTask;
        }
    }
}
