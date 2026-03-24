using Ozakboy.FCM.Models.Messages;
using Ozakboy.FCM.Models.Responses;

namespace Ozakboy.FCM.Interfaces
{
    /// <summary>
    /// FCM 推播通知服務介面
    /// 提供 Google Firebase Cloud Messaging HTTP v1 API 的完整功能
    /// </summary>
    public interface IFCMService
    {
        #region 單一裝置推播

        /// <summary>
        /// 發送通知到指定裝置
        /// </summary>
        /// <param name="token">裝置註冊 Token</param>
        /// <param name="notification">通知內容</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task<FCMResponse> SendAsync(string token, Notification notification, CancellationToken cancellationToken = default);

        /// <summary>
        /// 發送通知到指定裝置（附帶自訂資料）
        /// </summary>
        /// <param name="token">裝置註冊 Token</param>
        /// <param name="notification">通知內容</param>
        /// <param name="data">自訂 key-value 資料</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task<FCMResponse> SendAsync(string token, Notification notification, Dictionary<string, string> data, CancellationToken cancellationToken = default);

        /// <summary>
        /// 發送純資料訊息到指定裝置（靜默推播，不顯示通知）
        /// </summary>
        /// <param name="token">裝置註冊 Token</param>
        /// <param name="data">自訂 key-value 資料</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task<FCMResponse> SendDataAsync(string token, Dictionary<string, string> data, CancellationToken cancellationToken = default);

        /// <summary>
        /// 發送完全自訂的 FCM 訊息
        /// </summary>
        /// <param name="message">完整 FCM 訊息</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task<FCMResponse> SendAsync(FCMMessage message, CancellationToken cancellationToken = default);

        /// <summary>
        /// 驗證訊息格式（不實際發送）
        /// </summary>
        /// <param name="message">要驗證的訊息</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task<FCMResponse> ValidateAsync(FCMMessage message, CancellationToken cancellationToken = default);

        #endregion

        #region 主題推播

        /// <summary>
        /// 發送通知到主題
        /// </summary>
        /// <param name="topic">主題名稱</param>
        /// <param name="notification">通知內容</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task<FCMResponse> SendToTopicAsync(string topic, Notification notification, CancellationToken cancellationToken = default);

        /// <summary>
        /// 發送通知到主題（附帶自訂資料）
        /// </summary>
        /// <param name="topic">主題名稱</param>
        /// <param name="notification">通知內容</param>
        /// <param name="data">自訂 key-value 資料</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task<FCMResponse> SendToTopicAsync(string topic, Notification notification, Dictionary<string, string> data, CancellationToken cancellationToken = default);

        /// <summary>
        /// 發送純資料訊息到主題（靜默推播）
        /// </summary>
        /// <param name="topic">主題名稱</param>
        /// <param name="data">自訂 key-value 資料</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task<FCMResponse> SendDataToTopicAsync(string topic, Dictionary<string, string> data, CancellationToken cancellationToken = default);

        #endregion

        #region 條件推播

        /// <summary>
        /// 發送通知到符合條件的裝置
        /// 條件格式範例: "'TopicA' in topics &amp;&amp; 'TopicB' in topics"
        /// </summary>
        /// <param name="condition">條件運算式</param>
        /// <param name="notification">通知內容</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task<FCMResponse> SendToConditionAsync(string condition, Notification notification, CancellationToken cancellationToken = default);

        /// <summary>
        /// 發送通知到符合條件的裝置（附帶自訂資料）
        /// </summary>
        /// <param name="condition">條件運算式</param>
        /// <param name="notification">通知內容</param>
        /// <param name="data">自訂 key-value 資料</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task<FCMResponse> SendToConditionAsync(string condition, Notification notification, Dictionary<string, string> data, CancellationToken cancellationToken = default);

        #endregion

        #region 批次推播

        /// <summary>
        /// 批次發送通知到多個裝置
        /// </summary>
        /// <param name="tokens">裝置 Token 列表（最多 500 個）</param>
        /// <param name="notification">通知內容</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task<BatchResponse> SendMulticastAsync(List<string> tokens, Notification notification, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批次發送通知到多個裝置（附帶自訂資料）
        /// </summary>
        /// <param name="tokens">裝置 Token 列表（最多 500 個）</param>
        /// <param name="notification">通知內容</param>
        /// <param name="data">自訂 key-value 資料</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task<BatchResponse> SendMulticastAsync(List<string> tokens, Notification notification, Dictionary<string, string> data, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批次發送多個不同的 FCM 訊息
        /// </summary>
        /// <param name="messages">訊息列表（最多 500 個）</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task<BatchResponse> SendBatchAsync(List<FCMMessage> messages, CancellationToken cancellationToken = default);

        #endregion

        #region 主題管理

        /// <summary>
        /// 訂閱裝置到主題
        /// </summary>
        /// <param name="topic">主題名稱</param>
        /// <param name="tokens">要訂閱的裝置 Token 列表</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task<TopicResponse> SubscribeToTopicAsync(string topic, List<string> tokens, CancellationToken cancellationToken = default);

        /// <summary>
        /// 訂閱單一裝置到主題
        /// </summary>
        /// <param name="topic">主題名稱</param>
        /// <param name="token">裝置 Token</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task<TopicResponse> SubscribeToTopicAsync(string topic, string token, CancellationToken cancellationToken = default);

        /// <summary>
        /// 取消訂閱裝置的主題
        /// </summary>
        /// <param name="topic">主題名稱</param>
        /// <param name="tokens">要取消訂閱的裝置 Token 列表</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task<TopicResponse> UnsubscribeFromTopicAsync(string topic, List<string> tokens, CancellationToken cancellationToken = default);

        /// <summary>
        /// 取消訂閱單一裝置的主題
        /// </summary>
        /// <param name="topic">主題名稱</param>
        /// <param name="token">裝置 Token</param>
        /// <param name="cancellationToken">取消權杖</param>
        Task<TopicResponse> UnsubscribeFromTopicAsync(string topic, string token, CancellationToken cancellationToken = default);

        #endregion
    }
}
