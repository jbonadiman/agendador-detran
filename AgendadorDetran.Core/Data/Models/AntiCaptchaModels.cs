using System.Text.Json.Serialization;

namespace AgendadorDetran.Core.Data.Models
{
    public class CreateTaskRequest
    {
        [JsonPropertyName("clientKey")]
        public string? ClientKey { get; set; }
        
        [JsonPropertyName("task")]
        public RecaptchaTask? Task { get; set; }
    }

    public class RecaptchaTask
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "RecaptchaV2TaskProxyless";

        [JsonPropertyName("websiteURL")]
        public string WebsiteUrl { get; set; } = "http://www2.detran.rj.gov.br/portal/IdentificacaoCivil/agendamentoDados";
        
        [JsonPropertyName("websiteKey")]
        public string? WebsiteKey { get; set; }
    }

    public class CreateTaskResponse
    {
        [JsonPropertyName("errorId")]
        public int? ErrorId { get; set; }

        [JsonPropertyName("errorCode")]
        public string? ErrorCode { get; set; }
        
        [JsonPropertyName("errorDescription")]
        public string? ErrorDescription { get; set; }
        
        [JsonPropertyName("taskId")]
        public int? TaskId { get; set; }
    }
    
    public class GetTaskResultRequest
    {
        [JsonPropertyName("clientKey")]
        public string? ClientKey { get; set; }
        
        [JsonPropertyName("taskId")]
        public int TaskId { get; set; }
    }
    
    public class GetTaskResultResponse
    {
        [JsonPropertyName("errorId")]
        public int? ErrorId { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
        
        [JsonPropertyName("solution")]
        public RecaptchaSolution? RecaptchaSolution { get; set; }
        
        [JsonPropertyName("cost")]
        public string? Cost { get; set; }
        
        [JsonPropertyName("ip")]
        public string? Ip { get; set; }
        
        [JsonPropertyName("createTime")]
        public int CreateTime { get; set; }
        
        [JsonPropertyName("endTime")]
        public int EndTime { get; set; }
        
        [JsonPropertyName("solveCount")]
        public int SolveCount { get; set; }
    }
    
    public class RecaptchaSolution
    {
        [JsonPropertyName("gRecaptchaResponse")]
        public string Id { get; set; }
    }
}