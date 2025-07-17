using Astro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Winform.Net
{
    internal static class ResponseMessageExtensions
    {
        internal static async Task<ProblemDetails?> GetProbemDetails(this HttpResponseMessage response)
        {
            var problems = ProblemDetails.Create(await response.Content.ReadAsStringAsync());
            return problems;
        }
    }
}
