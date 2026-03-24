using System.Text.Json.Serialization;

namespace Ozakboy.FCM.Models.Messages
{
    /// <summary>
    /// FCM v1 API 訊息模型
    /// </summary>
    public class FCMMessage
    {
        /// <summary>
        /// 訊息名稱（由伺服器產生，格式: projects/*/messages/{message_id}）
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// 通知內容
        /// </summary>
        [JsonPropertyName("notification")]
        public Notification? Notification { get; set; }

        /// <summary>
        /// 自訂 key-value 資料（所有值必須為字串）
        /// </summary>
        [JsonPropertyName("data")]
        public Dictionary<string, string>? Data { get; set; }

        /// <summary>
        /// Android 平台專屬設定
        /// </summary>
        [JsonPropertyName("android")]
        public AndroidConfig? Android { get; set; }

        /// <summary>
        /// Apple (APNs) 平台專屬設定
        /// </summary>
        [JsonPropertyName("apns")]
        public ApnsConfig? Apns { get; set; }

        /// <summary>
        /// Web Push 平台專屬設定
        /// </summary>
        [JsonPropertyName("webpush")]
        public WebpushConfig? Webpush { get; set; }

        /// <summary>
        /// FCM 選項
        /// </summary>
        [JsonPropertyName("fcm_options")]
        public FcmMessageOptions? FcmOptions { get; set; }

        /// <summary>
        /// 目標裝置的註冊 Token（與 Topic、Condition 擇一使用）
        /// </summary>
        [JsonPropertyName("token")]
        public string? Token { get; set; }

        /// <summary>
        /// 目標主題名稱（與 Token、Condition 擇一使用）
        /// </summary>
        [JsonPropertyName("topic")]
        public string? Topic { get; set; }

        /// <summary>
        /// 目標條件運算式（與 Token、Topic 擇一使用）
        /// 例如: "'TopicA' in topics &amp;&amp; 'TopicB' in topics"
        /// </summary>
        [JsonPropertyName("condition")]
        public string? Condition { get; set; }
    }

    /// <summary>
    /// FCM 訊息選項
    /// </summary>
    public class FcmMessageOptions
    {
        /// <summary>
        /// 分析標籤（最多 50 字元）
        /// </summary>
        [JsonPropertyName("analytics_label")]
        public string? AnalyticsLabel { get; set; }
    }

    /// <summary>
    /// FCM v1 API 發送請求包裝
    /// </summary>
    internal class FCMSendRequest
    {
        /// <summary>
        /// 訊息內容
        /// </summary>
        [JsonPropertyName("message")]
        public FCMMessage Message { get; set; } = new();

        /// <summary>
        /// 是否只驗證不實際發送
        /// </summary>
        [JsonPropertyName("validate_only")]
        public bool? ValidateOnly { get; set; }
    }
}
