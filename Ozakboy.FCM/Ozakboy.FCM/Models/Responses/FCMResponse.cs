using System.Text.Json.Serialization;

namespace Ozakboy.FCM.Models.Responses
{
    /// <summary>
    /// FCM 發送回應
    /// </summary>
    public class FCMResponse
    {
        /// <summary>
        /// 是否發送成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 訊息名稱（成功時由 FCM 伺服器回傳，格式: projects/*/messages/{message_id}）
        /// </summary>
        [JsonPropertyName("name")]
        public string? MessageName { get; set; }

        /// <summary>
        /// HTTP 狀態碼
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// FCM 錯誤代碼
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// 原始回應內容
        /// </summary>
        public string? RawResponse { get; set; }
    }

    /// <summary>
    /// FCM 錯誤回應模型（對應 API 錯誤格式）
    /// </summary>
    internal class FCMErrorResponse
    {
        [JsonPropertyName("error")]
        public FCMError? Error { get; set; }
    }

    /// <summary>
    /// FCM 錯誤詳細資訊
    /// </summary>
    internal class FCMError
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("details")]
        public List<FCMErrorDetail>? Details { get; set; }
    }

    /// <summary>
    /// FCM 錯誤附加資訊
    /// </summary>
    internal class FCMErrorDetail
    {
        [JsonPropertyName("@type")]
        public string? Type { get; set; }

        [JsonPropertyName("errorCode")]
        public string? ErrorCode { get; set; }
    }
}
