using System.Text.Json.Serialization;

namespace Ozakboy.FCM.Models.Messages
{
    /// <summary>
    /// Android 平台專屬設定
    /// </summary>
    public class AndroidConfig
    {
        /// <summary>
        /// 訊息折疊鍵（同一 collapse_key 的訊息會被新的取代）
        /// </summary>
        [JsonPropertyName("collapse_key")]
        public string? CollapseKey { get; set; }

        /// <summary>
        /// 訊息優先級
        /// </summary>
        [JsonPropertyName("priority")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AndroidMessagePriority? Priority { get; set; }

        /// <summary>
        /// 訊息存活時間 (例如 "3.5s" 表示 3.5 秒)
        /// </summary>
        [JsonPropertyName("ttl")]
        public string? Ttl { get; set; }

        /// <summary>
        /// 限定接收的套件名稱
        /// </summary>
        [JsonPropertyName("restricted_package_name")]
        public string? RestrictedPackageName { get; set; }

        /// <summary>
        /// 自訂 key-value 資料
        /// </summary>
        [JsonPropertyName("data")]
        public Dictionary<string, string>? Data { get; set; }

        /// <summary>
        /// Android 通知設定
        /// </summary>
        [JsonPropertyName("notification")]
        public AndroidNotification? Notification { get; set; }

        /// <summary>
        /// 是否為直接啟動模式訊息
        /// </summary>
        [JsonPropertyName("direct_boot_ok")]
        public bool? DirectBootOk { get; set; }
    }

    /// <summary>
    /// Android 訊息優先級
    /// </summary>
    public enum AndroidMessagePriority
    {
        /// <summary>
        /// 一般優先級
        /// </summary>
        [JsonPropertyName("normal")]
        Normal,

        /// <summary>
        /// 高優先級
        /// </summary>
        [JsonPropertyName("high")]
        High
    }

    /// <summary>
    /// Android 通知詳細設定
    /// </summary>
    public class AndroidNotification
    {
        /// <summary>
        /// 通知標題（覆寫全域 Notification.Title）
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        /// <summary>
        /// 通知內文（覆寫全域 Notification.Body）
        /// </summary>
        [JsonPropertyName("body")]
        public string? Body { get; set; }

        /// <summary>
        /// 通知圖示
        /// </summary>
        [JsonPropertyName("icon")]
        public string? Icon { get; set; }

        /// <summary>
        /// 通知顏色 (格式: #rrggbb)
        /// </summary>
        [JsonPropertyName("color")]
        public string? Color { get; set; }

        /// <summary>
        /// 通知音效
        /// </summary>
        [JsonPropertyName("sound")]
        public string? Sound { get; set; }

        /// <summary>
        /// 通知標籤（用於取代現有通知）
        /// </summary>
        [JsonPropertyName("tag")]
        public string? Tag { get; set; }

        /// <summary>
        /// 點擊通知時的動作
        /// </summary>
        [JsonPropertyName("click_action")]
        public string? ClickAction { get; set; }

        /// <summary>
        /// 通知頻道 ID (Android O+)
        /// </summary>
        [JsonPropertyName("channel_id")]
        public string? ChannelId { get; set; }

        /// <summary>
        /// 通知圖片網址
        /// </summary>
        [JsonPropertyName("image")]
        public string? Image { get; set; }

        /// <summary>
        /// 通知優先級
        /// </summary>
        [JsonPropertyName("notification_priority")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NotificationPriority? NotificationPriority { get; set; }

        /// <summary>
        /// 通知數量
        /// </summary>
        [JsonPropertyName("notification_count")]
        public int? NotificationCount { get; set; }

        /// <summary>
        /// 是否預設開啟音效
        /// </summary>
        [JsonPropertyName("default_sound")]
        public bool? DefaultSound { get; set; }

        /// <summary>
        /// 是否預設開啟震動
        /// </summary>
        [JsonPropertyName("default_vibrate_timings")]
        public bool? DefaultVibrateTimings { get; set; }

        /// <summary>
        /// 是否預設開啟燈光
        /// </summary>
        [JsonPropertyName("default_light_settings")]
        public bool? DefaultLightSettings { get; set; }
    }

    /// <summary>
    /// Android 通知優先級
    /// </summary>
    public enum NotificationPriority
    {
        PRIORITY_UNSPECIFIED,
        PRIORITY_MIN,
        PRIORITY_LOW,
        PRIORITY_DEFAULT,
        PRIORITY_HIGH,
        PRIORITY_MAX
    }
}
