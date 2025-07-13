using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class CommonResult
    {
        [JsonConstructor] internal CommonResult(bool success, string message = "")
        {
            Success = success;
            Message = message;
        }
        [JsonPropertyName("success")] public bool Success { get; }
        [JsonPropertyName("message")] public string Message { get; } = string.Empty;

        public static CommonResult Ok(string message = "") => new CommonResult(true, message);
        public static CommonResult Fail(string message = "") => new CommonResult(false, message);

        public static CommonResult? Create(string json) => string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<CommonResult>(json, AppJsonSerializerContext.Default.CommonResult);
        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.CommonResult);
    }
}
