using System.Text.Json.Serialization;

namespace Ozakboy.FCM.Models.Messages
{
    /// <summary>
    /// Web Push 平台專屬設定
    /// </summary>
    public class WebpushConfig
    {
        /// <summary>
        /// Web Push 請求標頭
        /// </summary>
        [JsonPropertyName("headers")]
        public Dictionary<string, string>? Headers { get; set; }

        /// <summary>
        /// 自訂 key-value 資料
        /// </summary>
        [JsonPropertyName("data")]
        public Dictionary<string, string>? Data { get; set; }

        /// <summary>
        /// Web Push 通知設定
        /// </summary>
        [JsonPropertyName("notification")]
        public WebpushNotification? Notification { get; set; }

        /// <summary>
        /// FCM 選項
        /// </summary>
        [JsonPropertyName("fcm_options")]
        public WebpushFcmOptions? FcmOptions { get; set; }
    }

    /// <summary>
    /// Web Push 通知詳細設定
    /// </summary>
    public class WebpushNotification
    {
        /// <summary>
        /// 通知標題
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        /// <summary>
        /// 通知內文
        /// </summary>
        [JsonPropertyName("body")]
        public string? Body { get; set; }

        /// <summary>
        /// 通知圖示網址
        /// </summary>
        [JsonPropertyName("icon")]
        public string? Icon { get; set; }

        /// <summary>
        /// 通知圖片網址
        /// </summary>
        [JsonPropertyName("image")]
        public string? Image { get; set; }

        /// <summary>
        /// 角標網址
        /// </summary>
        [JsonPropertyName("badge")]
        public string? Badge { get; set; }

        /// <summary>
        /// 震動模式（毫秒陣列）
        /// </summary>
        [JsonPropertyName("vibrate")]
        public int[]? Vibrate { get; set; }

        /// <summary>
        /// 通知方向
        /// </summary>
        [JsonPropertyName("dir")]
        public string? Dir { get; set; }

        /// <summary>
        /// 通知標籤
        /// </summary>
        [JsonPropertyName("tag")]
        public string? Tag { get; set; }

        /// <summary>
        /// 通知語言
        /// </summary>
        [JsonPropertyName("lang")]
        public string? Lang { get; set; }

        /// <summary>
        /// 是否重複顯示
        /// </summary>
        [JsonPropertyName("renotify")]
        public bool? Renotify { get; set; }

        /// <summary>
        /// 是否需要互動
        /// </summary>
        [JsonPropertyName("requireInteraction")]
        public bool? RequireInteraction { get; set; }

        /// <summary>
        /// 是否靜音
        /// </summary>
        [JsonPropertyName("silent")]
        public bool? Silent { get; set; }

        /// <summary>
        /// 通知時間戳
        /// </summary>
        [JsonPropertyName("timestamp")]
        public long? Timestamp { get; set; }

        /// <summary>
        /// 操作按鈕
        /// </summary>
        [JsonPropertyName("actions")]
        public List<WebpushNotificationAction>? Actions { get; set; }
    }

    /// <summary>
    /// Web Push 通知操作按鈕
    /// </summary>
    public class WebpushNotificationAction
    {
        /// <summary>
        /// 動作識別碼
        /// </summary>
        [JsonPropertyName("action")]
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// 按鈕標題
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 按鈕圖示網址
        /// </summary>
        [JsonPropertyName("icon")]
        public string? Icon { get; set; }
    }

    /// <summary>
    /// Web Push FCM 選項
    /// </summary>
    public class WebpushFcmOptions
    {
        /// <summary>
        /// 點擊通知時開啟的網址
        /// </summary>
        [JsonPropertyName("link")]
        public string? Link { get; set; }

        /// <summary>
        /// 分析標籤
        /// </summary>
        [JsonPropertyName("analytics_label")]
        public string? AnalyticsLabel { get; set; }
    }
}
