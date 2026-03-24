using Microsoft.AspNetCore.Mvc;
using Ozakboy.FCM.Builders;
using Ozakboy.FCM.Interfaces;
using Ozakboy.FCM.Models.Messages;

namespace Ozakboy.FCM.WebApiTest.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FCMTestController : ControllerBase
    {
        private readonly IFCMService _fcmService;
        private const string TestToken = "your_device_token_here";

        public FCMTestController(IFCMService fcmService)
        {
            _fcmService = fcmService;
        }

        /// <summary>
        /// 測試發送單一裝置通知
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TestSend()
        {
            var result = await _fcmService.SendAsync(TestToken,
                new Notification { Title = "測試", Body = "發送通知" });
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 測試發送通知（附帶自訂資料）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TestSendWithData()
        {
            var result = await _fcmService.SendAsync(TestToken,
                new Notification { Title = "訂單通知", Body = "您的訂單已出貨" },
                new Dictionary<string, string>
                {
                    { "order_id", "12345" },
                    { "action", "open_order" }
                });
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 測試發送靜默推播（純資料，不顯示通知）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TestSendData()
        {
            var result = await _fcmService.SendDataAsync(TestToken,
                new Dictionary<string, string>
                {
                    { "sync", "true" },
                    { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() }
                });
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 測試使用 Fluent Builder 建構訊息
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TestSendWithBuilder()
        {
            // 使用 Fluent API 鏈式語法建構訊息
            var message = FCMMessageBuilder.Create()
                .ToToken(TestToken)
                .WithNotification("促銷活動", "限時 8 折優惠！", "https://example.com/promo.jpg")
                .AddData("url", "/promo/summer2024")
                .WithHighPriority()
                .WithSound()
                .WithBadge(1)
                .WithChannelId("promotions")
                .WithWebLink("https://example.com/promo")
                .WithTimeToLive(TimeSpan.FromHours(24))
                .WithCollapseKey("promo_2024")
                .Build();

            var result = await _fcmService.SendAsync(message);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 測試發送完全自訂訊息（含 Android/iOS/Web 平台設定）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TestSendCustom()
        {
            var message = new FCMMessage
            {
                Token = TestToken,
                Notification = new Notification
                {
                    Title = "促銷活動",
                    Body = "限時 8 折優惠！",
                    Image = "https://example.com/promo.jpg"
                },
                Data = new Dictionary<string, string>
                {
                    { "url", "/promo/summer2024" }
                },
                Android = new AndroidConfig
                {
                    Priority = AndroidMessagePriority.High,
                    Notification = new AndroidNotification
                    {
                        ChannelId = "promotions",
                        Sound = "default",
                        ClickAction = "OPEN_PROMO"
                    }
                },
                Apns = new ApnsConfig
                {
                    Payload = new ApnsPayload
                    {
                        Aps = new Aps
                        {
                            Badge = 1,
                            Sound = "default"
                        }
                    }
                },
                Webpush = new WebpushConfig
                {
                    FcmOptions = new WebpushFcmOptions
                    {
                        Link = "https://example.com/promo"
                    }
                }
            };

            var result = await _fcmService.SendAsync(message);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 測試訂閱主題
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> SubscribeTopic()
        {
            var result = await _fcmService.SubscribeToTopicAsync("UserEnd", TestToken);
            return result.IsAllSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 測試取消訂閱主題
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> UnsubscribeTopic()
        {
            var result = await _fcmService.UnsubscribeFromTopicAsync("UserEnd", TestToken);
            return result.IsAllSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 測試發送主題通知
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> SendToTopic()
        {
            var result = await _fcmService.SendToTopicAsync("UserEnd",
                new Notification { Title = "主題通知", Body = "這是發送給 UserEnd 主題的訊息" });
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 測試條件推播
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> SendToCondition()
        {
            var result = await _fcmService.SendToConditionAsync(
                "'UserEnd' in topics || 'AdminEnd' in topics",
                new Notification { Title = "條件推播", Body = "這是條件推播訊息" });
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 測試批次推播（支援超過 500 筆自動分批）
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SendMulticast([FromBody] List<string> tokens)
        {
            if (tokens == null || tokens.Count == 0)
                return BadRequest("請提供 Token 列表");

            var result = await _fcmService.SendMulticastAsync(tokens,
                new Notification { Title = "批次通知", Body = "這是批次推播訊息" });

            // 檢查是否有失效的 Token
            if (result.HasUnregisteredTokens)
            {
                // 在實際應用中，這裡可以將失效的 Token 從資料庫移除
                // await _tokenRepository.RemoveTokensAsync(result.UnregisteredTokens);
            }

            return Ok(new
            {
                result.SuccessCount,
                result.FailureCount,
                result.TotalCount,
                result.IsAllSuccess,
                result.UnregisteredTokens,
                result.HasUnregisteredTokens,
                Results = result.Results
            });
        }

        /// <summary>
        /// 測試排程推播（延遲 10 秒後發送）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TestScheduledSend()
        {
            var result = await _fcmService.SendScheduledAsync(
                TestToken,
                new Notification { Title = "排程通知", Body = "這是延遲 10 秒後發送的訊息" },
                TimeSpan.FromSeconds(10));
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 測試排程推播（指定時間發送）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TestScheduledSendAt()
        {
            // 使用 Builder 建構訊息，並在 1 分鐘後發送
            var message = FCMMessageBuilder.Create()
                .ToToken(TestToken)
                .WithNotification("定時通知", "這是指定時間發送的訊息")
                .WithHighPriority()
                .Build();

            var sendAt = DateTimeOffset.UtcNow.AddMinutes(1);
            var result = await _fcmService.SendScheduledAsync(message, sendAt);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 測試驗證訊息格式（不實際發送）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ValidateMessage()
        {
            var message = new FCMMessage
            {
                Token = TestToken,
                Notification = new Notification { Title = "驗證", Body = "驗證訊息格式" }
            };

            var result = await _fcmService.ValidateAsync(message);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
