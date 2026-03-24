using System.Text.Json.Serialization;

namespace Ozakboy.FCM.Models.Messages
{
    /// <summary>
    /// FCM 通知內容
    /// </summary>
    public class Notification
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
        /// 通知圖片網址
        /// </summary>
        [JsonPropertyName("image")]
        public string? Image { get; set; }
    }
}
