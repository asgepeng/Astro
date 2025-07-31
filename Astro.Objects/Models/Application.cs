using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Session
    {
        [JsonPropertyName("sessionId")]
        public Guid SessionId { get; set; } = Guid.NewGuid();
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("startTime")]
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        [JsonPropertyName("endTime")]
        public DateTime? EndTime { get; set; } = null;
        [JsonPropertyName("isActive")]
        public bool IsActive => EndTime == null || EndTime > DateTime.UtcNow;
    }
    public class Application
    {
        [JsonPropertyName("appId")]
        public Guid AppId { get; set; } = Guid.NewGuid();
        [JsonPropertyName("appName")]
        public string AppName { get; set; } = string.Empty;
        [JsonPropertyName("appAcronym")]
        public string AppAcronym { get; set; } = string.Empty;
        [JsonPropertyName("appVersion")]
        public string AppVersion { get; set; } = "1.0.0";
        [JsonPropertyName("appDescription")]
        public string AppDescription { get; set; } = string.Empty;
        [JsonPropertyName("appIcon")]
        public string AppIcon { get; set; } = string.Empty;
        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; } = null;
        [JsonPropertyName("sessions")]
        public List<Session> Sessions { get; set; } = new List<Session>();
    }
}
