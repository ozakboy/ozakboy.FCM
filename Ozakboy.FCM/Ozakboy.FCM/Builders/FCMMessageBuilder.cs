using Ozakboy.FCM.Models.Messages;

namespace Ozakboy.FCM.Builders
{
    /// <summary>
    /// FCM 訊息建構器 - 提供 Fluent API 鏈式語法建構 FCM 訊息
    /// </summary>
    public class FCMMessageBuilder
    {
        private readonly FCMMessage _message = new();

        /// <summary>
        /// 建立新的訊息建構器
        /// </summary>
        public static FCMMessageBuilder Create() => new();

        #region 目標設定

        /// <summary>
        /// 設定目標裝置 Token
        /// </summary>
        public FCMMessageBuilder ToToken(string token)
        {
            _message.Token = token;
            return this;
        }

        /// <summary>
        /// 設定目標主題
        /// </summary>
        public FCMMessageBuilder ToTopic(string topic)
        {
            _message.Topic = topic;
            return this;
        }

        /// <summary>
        /// 設定目標條件運算式
        /// </summary>
        public FCMMessageBuilder ToCondition(string condition)
        {
            _message.Condition = condition;
            return this;
        }

        #endregion

        #region 通知內容

        /// <summary>
        /// 設定通知標題與內文
        /// </summary>
        public FCMMessageBuilder WithNotification(string title, string body, string? image = null)
        {
            _message.Notification = new Notification
            {
                Title = title,
                Body = body,
                Image = image
            };
            return this;
        }

        /// <summary>
        /// 設定通知物件
        /// </summary>
        public FCMMessageBuilder WithNotification(Notification notification)
        {
            _message.Notification = notification;
            return this;
        }

        /// <summary>
        /// 設定自訂資料
        /// </summary>
        public FCMMessageBuilder WithData(Dictionary<string, string> data)
        {
            _message.Data = data;
            return this;
        }

        /// <summary>
        /// 新增單筆自訂資料
        /// </summary>
        public FCMMessageBuilder AddData(string key, string value)
        {
            _message.Data ??= new Dictionary<string, string>();
            _message.Data[key] = value;
            return this;
        }

        #endregion

        #region 平台設定

        /// <summary>
        /// 設定 Android 平台配置
        /// </summary>
        public FCMMessageBuilder WithAndroid(AndroidConfig android)
        {
            _message.Android = android;
            return this;
        }

        /// <summary>
        /// 設定 Android 平台配置（使用委派）
        /// </summary>
        public FCMMessageBuilder WithAndroid(Action<AndroidConfig> configure)
        {
            _message.Android ??= new AndroidConfig();
            configure(_message.Android);
            return this;
        }

        /// <summary>
        /// 設定 APNs (iOS) 平台配置
        /// </summary>
        public FCMMessageBuilder WithApns(ApnsConfig apns)
        {
            _message.Apns = apns;
            return this;
        }

        /// <summary>
        /// 設定 APNs (iOS) 平台配置（使用委派）
        /// </summary>
        public FCMMessageBuilder WithApns(Action<ApnsConfig> configure)
        {
            _message.Apns ??= new ApnsConfig();
            configure(_message.Apns);
            return this;
        }

        /// <summary>
        /// 設定 Web Push 平台配置
        /// </summary>
        public FCMMessageBuilder WithWebpush(WebpushConfig webpush)
        {
            _message.Webpush = webpush;
            return this;
        }

        /// <summary>
        /// 設定 Web Push 平台配置（使用委派）
        /// </summary>
        public FCMMessageBuilder WithWebpush(Action<WebpushConfig> configure)
        {
            _message.Webpush ??= new WebpushConfig();
            configure(_message.Webpush);
            return this;
        }

        #endregion

        #region 進階設定

        /// <summary>
        /// 設定 FCM 選項（分析標籤）
        /// </summary>
        public FCMMessageBuilder WithAnalyticsLabel(string label)
        {
            _message.FcmOptions = new FcmMessageOptions { AnalyticsLabel = label };
            return this;
        }

        /// <summary>
        /// 設定訊息存活時間（跨平台統一設定）
        /// 會同時設定 Android TTL 和 APNs expiration 及 WebPush TTL
        /// </summary>
        public FCMMessageBuilder WithTimeToLive(TimeSpan ttl)
        {
            // Android TTL (格式: "3.5s")
            _message.Android ??= new AndroidConfig();
            _message.Android.Ttl = $"{ttl.TotalSeconds}s";

            // APNs expiration header
            _message.Apns ??= new ApnsConfig();
            _message.Apns.Headers ??= new Dictionary<string, string>();
            var expirationTimestamp = DateTimeOffset.UtcNow.Add(ttl).ToUnixTimeSeconds();
            _message.Apns.Headers["apns-expiration"] = expirationTimestamp.ToString();

            // WebPush TTL header
            _message.Webpush ??= new WebpushConfig();
            _message.Webpush.Headers ??= new Dictionary<string, string>();
            _message.Webpush.Headers["TTL"] = ((int)ttl.TotalSeconds).ToString();

            return this;
        }

        /// <summary>
        /// 設定 Android 高優先級推播
        /// </summary>
        public FCMMessageBuilder WithHighPriority()
        {
            _message.Android ??= new AndroidConfig();
            _message.Android.Priority = AndroidMessagePriority.High;

            _message.Apns ??= new ApnsConfig();
            _message.Apns.Headers ??= new Dictionary<string, string>();
            _message.Apns.Headers["apns-priority"] = "10";

            _message.Webpush ??= new WebpushConfig();
            _message.Webpush.Headers ??= new Dictionary<string, string>();
            _message.Webpush.Headers["Urgency"] = "high";

            return this;
        }

        /// <summary>
        /// 設定訊息折疊鍵（同一 key 的新訊息會取代舊訊息）
        /// </summary>
        public FCMMessageBuilder WithCollapseKey(string collapseKey)
        {
            _message.Android ??= new AndroidConfig();
            _message.Android.CollapseKey = collapseKey;

            _message.Apns ??= new ApnsConfig();
            _message.Apns.Headers ??= new Dictionary<string, string>();
            _message.Apns.Headers["apns-collapse-id"] = collapseKey;

            _message.Webpush ??= new WebpushConfig();
            _message.Webpush.Headers ??= new Dictionary<string, string>();
            _message.Webpush.Headers["Topic"] = collapseKey;

            return this;
        }

        /// <summary>
        /// 設定 iOS 角標數字
        /// </summary>
        public FCMMessageBuilder WithBadge(int badge)
        {
            _message.Apns ??= new ApnsConfig();
            _message.Apns.Payload ??= new ApnsPayload();
            _message.Apns.Payload.Aps ??= new Aps();
            _message.Apns.Payload.Aps.Badge = badge;
            return this;
        }

        /// <summary>
        /// 設定 Android 通知頻道
        /// </summary>
        public FCMMessageBuilder WithChannelId(string channelId)
        {
            _message.Android ??= new AndroidConfig();
            _message.Android.Notification ??= new AndroidNotification();
            _message.Android.Notification.ChannelId = channelId;
            return this;
        }

        /// <summary>
        /// 設定預設音效（跨平台）
        /// </summary>
        public FCMMessageBuilder WithSound(string sound = "default")
        {
            _message.Android ??= new AndroidConfig();
            _message.Android.Notification ??= new AndroidNotification();
            _message.Android.Notification.Sound = sound;

            _message.Apns ??= new ApnsConfig();
            _message.Apns.Payload ??= new ApnsPayload();
            _message.Apns.Payload.Aps ??= new Aps();
            _message.Apns.Payload.Aps.Sound = sound;

            return this;
        }

        /// <summary>
        /// 設定 Web Push 點擊連結
        /// </summary>
        public FCMMessageBuilder WithWebLink(string link)
        {
            _message.Webpush ??= new WebpushConfig();
            _message.Webpush.FcmOptions ??= new WebpushFcmOptions();
            _message.Webpush.FcmOptions.Link = link;
            return this;
        }

        #endregion

        /// <summary>
        /// 建構 FCM 訊息
        /// </summary>
        public FCMMessage Build()
        {
            return _message;
        }
    }
}
