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

    public class Reference
    {
        [JsonPropertyName("referenceId")]
        public Guid ReferenceId { get; set; } = Guid.NewGuid();

        [JsonPropertyName("applicantId")]
        public Guid ApplicantId { get; set; } // relasi ke pelamar

        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("relationship")]
        public string Relationship { get; set; } = string.Empty; // contoh: "Former Manager", "Lecturer", "Colleague"

        [JsonPropertyName("organization")]
        public string Organization { get; set; } = string.Empty;

        [JsonPropertyName("position")]
        public string Position { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonPropertyName("note")]
        public string Note { get; set; } = string.Empty; // bisa diisi catatan atau pesan

        [JsonPropertyName("verified")]
        public bool Verified { get; set; } = false; // apakah sudah diverifikasi oleh sistem

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}
