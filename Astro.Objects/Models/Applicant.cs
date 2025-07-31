using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Applicant
    {
        [JsonPropertyName("applicantId")]
        public Guid ApplicantId { get; set; } = Guid.NewGuid();
    }
}
