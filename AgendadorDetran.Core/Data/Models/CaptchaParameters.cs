using Newtonsoft.Json;

namespace AgendadorDetran.Core.Data.Models
{
    public class CaptchaParameters
    {
        [JsonProperty("clientKey")]
        public string ClientKey { get; set; }
        
        [JsonProperty("task")]
        public CaptchaTask Task { get; set; }
    }

    public class CaptchaTask
    {
        [JsonProperty("type")] public const string Type = "RecaptchaV2TaskProxyless";
        
        [JsonProperty("websiteURL")]
        public string WebsiteUrl { get; set; }
        
        [JsonProperty("websiteKey")]
        public string WebsiteKey { get; set; }
    }
}