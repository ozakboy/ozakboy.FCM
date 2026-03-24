using System.Text.Json.Serialization;

namespace Ozakboy.FCM.Models.Responses
{
    /// <summary>
    /// 主題操作回應
    /// </summary>
    public class TopicResponse
    {
        /// <summary>
        /// 是否全部成功
        /// </summary>
        public bool IsAllSuccess => Results == null || Results.All(r => r == null);

        /// <summary>
        /// 成功數量
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// 失敗數量
        /// </summary>
        public int FailureCount { get; set; }

        /// <summary>
        /// 各筆結果（null 表示成功，非 null 表示錯誤）
        /// </summary>
        [JsonPropertyName("results")]
        public List<TopicResultError?>? Results { get; set; }

        /// <summary>
        /// HTTP 狀態碼
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// 錯誤訊息（整體失敗時）
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// 原始回應內容
        /// </summary>
        public string? RawResponse { get; set; }
    }

    /// <summary>
    /// 主題操作錯誤
    /// </summary>
    public class TopicResultError
    {
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }
}
