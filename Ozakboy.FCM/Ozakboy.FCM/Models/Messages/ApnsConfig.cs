using System.Text.Json.Serialization;

namespace Ozakboy.FCM.Models.Messages
{
    /// <summary>
    /// Apple Push Notification Service (APNs) 平台專屬設定
    /// </summary>
    public class ApnsConfig
    {
        /// <summary>
        /// APNs 請求標頭
        /// </summary>
        [JsonPropertyName("headers")]
        public Dictionary<string, string>? Headers { get; set; }

        /// <summary>
        /// APNs payload
        /// </summary>
        [JsonPropertyName("payload")]
        public ApnsPayload? Payload { get; set; }

        /// <summary>
        /// FCM 選項
        /// </summary>
        [JsonPropertyName("fcm_options")]
        public ApnsFcmOptions? FcmOptions { get; set; }
    }

    /// <summary>
    /// APNs Payload
    /// </summary>
    public class ApnsPayload
    {
        /// <summary>
        /// APS 字典
        /// </summary>
        [JsonPropertyName("aps")]
        public Aps? Aps { get; set; }
    }

    /// <summary>
    /// APNs APS 設定
    /// </summary>
    public class Aps
    {
        /// <summary>
        /// 警示內容
        /// </summary>
        [JsonPropertyName("alert")]
        public ApsAlert? Alert { get; set; }

        /// <summary>
        /// 角標數字
        /// </summary>
        [JsonPropertyName("badge")]
        public int? Badge { get; set; }

        /// <summary>
        /// 音效名稱
        /// </summary>
        [JsonPropertyName("sound")]
        public string? Sound { get; set; }

        /// <summary>
        /// 是否為背景推播 (設為 1 啟用)
        /// </summary>
        [JsonPropertyName("content-available")]
        public int? ContentAvailable { get; set; }

        /// <summary>
        /// 是否為可修改通知 (設為 1 啟用)
        /// </summary>
        [JsonPropertyName("mutable-content")]
        public int? MutableContent { get; set; }

        /// <summary>
        /// 通知分類 (對應 UNNotificationCategory)
        /// </summary>
        [JsonPropertyName("category")]
        public string? Category { get; set; }

        /// <summary>
        /// 執行緒 ID (用於通知分組)
        /// </summary>
        [JsonPropertyName("thread-id")]
        public string? ThreadId { get; set; }
    }

    /// <summary>
    /// APNs 警示內容
    /// </summary>
    public class ApsAlert
    {
        /// <summary>
        /// 警示標題
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        /// <summary>
        /// 警示副標題
        /// </summary>
        [JsonPropertyName("subtitle")]
        public string? Subtitle { get; set; }

        /// <summary>
        /// 警示內文
        /// </summary>
        [JsonPropertyName("body")]
        public string? Body { get; set; }
    }

    /// <summary>
    /// APNs FCM 選項
    /// </summary>
    public class ApnsFcmOptions
    {
        /// <summary>
        /// 分析標籤
        /// </summary>
        [JsonPropertyName("analytics_label")]
        public string? AnalyticsLabel { get; set; }

        /// <summary>
        /// 通知圖片網址
        /// </summary>
        [JsonPropertyName("image")]
        public string? Image { get; set; }
    }
}
