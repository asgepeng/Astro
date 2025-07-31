using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    using System;
    using System.Text.Json.Serialization;

    public class Applicant
    {
        [JsonPropertyName("applicantId")]
        public Guid ApplicantId { get; set; } = Guid.NewGuid();

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("fullName")]
        public string FullName => $"{FirstName} {LastName}".Trim();

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonPropertyName("dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; } = string.Empty; // Misalnya: Male, Female, Other

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("province")]
        public string Province { get; set; } = string.Empty;

        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; set; } = "Indonesia";

        [JsonPropertyName("identityNumber")]
        public string IdentityNumber { get; set; } = string.Empty; // KTP/NIK/Passport

        [JsonPropertyName("educationLevel")]
        public string EducationLevel { get; set; } = string.Empty; // SMA, D3, S1, S2, dll

        [JsonPropertyName("major")]
        public string Major { get; set; } = string.Empty;

        [JsonPropertyName("university")]
        public string University { get; set; } = string.Empty;

        [JsonPropertyName("graduationYear")]
        public int? GraduationYear { get; set; }

        [JsonPropertyName("experienceYears")]
        public double ExperienceYears { get; set; } = 0.0;

        [JsonPropertyName("currentJobTitle")]
        public string CurrentJobTitle { get; set; } = string.Empty;

        [JsonPropertyName("currentCompany")]
        public string CurrentCompany { get; set; } = string.Empty;

        [JsonPropertyName("expectedSalary")]
        public decimal? ExpectedSalary { get; set; }

        [JsonPropertyName("resumeUrl")]
        public string ResumeUrl { get; set; } = string.Empty;

        [JsonPropertyName("photoUrl")]
        public string PhotoUrl { get; set; } = string.Empty;

        [JsonPropertyName("applicationDate")]
        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Pending"; 
    }
}
