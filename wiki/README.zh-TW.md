[![nuget](https://img.shields.io/badge/nuget-ozakboy.FCM-blue)](https://www.nuget.org/packages/Ozakboy.FCM/) [![github](https://img.shields.io/badge/github-ozakboy.FCM-blue)](https://github.com/ozakboy/ozakboy.FCM/)

## 語言 / Language

- [English](../README.md)

## 要求

- .NET 6.0 / .NET 7.0 / .NET 8.0 / .NET 9.0

## 功能

完整整合 Google Firebase Cloud Messaging (FCM) HTTP v1 API 的 .NET 推播通知套件：

- 單一裝置推播通知
- 主題 (Topic) 推播通知
- 條件 (Condition) 推播通知
- 批次 / 多播 (Multicast) 推播通知（超過 500 筆自動分批）
- 排程推播通知（延遲時間或指定時間）
- 主題訂閱 / 取消訂閱管理
- 純資料靜默推播 (Data-only)
- 訊息格式驗證（不實際發送）
- Android / iOS (APNs) / Web Push 平台專屬設定
- Fluent 訊息建構器 API
- OAuth2 Service Account 認證，自動 Token 快取與刷新
- 指數退避重試機制，支援 Retry-After 標頭
- 使用 ozakboy.NLOG 完整日誌記錄

## 安裝

```
dotnet add package Ozakboy.FCM
```

## 設定

### 1. 取得 Firebase Service Account 金鑰

前往 [Firebase Console](https://console.firebase.google.com/) → 專案設定 → 服務帳戶 → 產生新的私密金鑰

### 2. 設定 appsettings.json

**方式一：指定金鑰檔案路徑**
```json
{
  "FCMSettings": {
    "ProjectId": "your-firebase-project-id",
    "ServiceAccountKeyPath": "path/to/serviceAccountKey.json"
  }
}
```

**方式二：直接嵌入金鑰內容**
```json
{
  "FCMSettings": {
    "ProjectId": "your-firebase-project-id",
    "ServiceAccountKey": {
      "type": "service_account",
      "project_id": "your-project-id",
      "private_key_id": "key-id",
      "private_key": "-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----\n",
      "client_email": "firebase-adminsdk-xxxxx@your-project.iam.gserviceaccount.com",
      "client_id": "123456789",
      "auth_uri": "https://accounts.google.com/o/oauth2/auth",
      "token_uri": "https://oauth2.googleapis.com/token",
      "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
      "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/..."
    }
  }
}
```

**重試設定（選填）**
```json
{
  "FCMSettings": {
    "ProjectId": "your-firebase-project-id",
    "ServiceAccountKeyPath": "serviceAccountKey.json",
    "Retry": {
      "Enabled": true,
      "MaxRetryCount": 3,
      "InitialDelayMs": 1000,
      "MaxDelayMs": 30000
    }
  }
}
```

### 3. 註冊服務 (Program.cs)

```csharp
using Ozakboy.FCM.Extensions;

// 方式一：從 Configuration 讀取
builder.Services.AddOzakboyFCM(builder.Configuration);

// 方式二：程式碼設定
builder.Services.AddOzakboyFCM(options =>
{
    options.ProjectId = "your-firebase-project-id";
    options.ServiceAccountKeyPath = "serviceAccountKey.json";
});
```

## 使用方式

### 注入服務

```csharp
using Ozakboy.FCM.Interfaces;
using Ozakboy.FCM.Models.Messages;

public class NotifyController : ControllerBase
{
    private readonly IFCMService _fcmService;

    public NotifyController(IFCMService fcmService)
    {
        _fcmService = fcmService;
    }
}
```

### 發送通知到單一裝置

```csharp
var result = await _fcmService.SendAsync("device_token",
    new Notification { Title = "標題", Body = "內容" });

if (result.IsSuccess)
    Console.WriteLine($"發送成功: {result.MessageName}");
else
    Console.WriteLine($"發送失敗: {result.ErrorMessage}");
```

### 發送通知（附帶自訂資料）

```csharp
var result = await _fcmService.SendAsync("device_token",
    new Notification { Title = "訂單通知", Body = "您的訂單已出貨" },
    new Dictionary<string, string>
    {
        { "order_id", "12345" },
        { "action", "open_order" }
    });
```

### 發送靜默推播（純資料，不顯示通知）

```csharp
var result = await _fcmService.SendDataAsync("device_token",
    new Dictionary<string, string>
    {
        { "sync", "true" },
        { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() }
    });
```

### 發送完全自訂訊息（含平台專屬設定）

```csharp
var message = new FCMMessage
{
    Token = "device_token",
    Notification = new Notification
    {
        Title = "促銷活動",
        Body = "限時 8 折優惠！",
        Image = "https://example.com/promo.jpg"
    },
    Data = new Dictionary<string, string>
    {
        { "url", "/promo/123" }
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
```

### 使用 Fluent 訊息建構器

```csharp
using Ozakboy.FCM.Builders;

var message = FCMMessageBuilder.Create()
    .ToToken("device_token")
    .WithNotification("促銷活動", "限時優惠！")
    .AddData("promo_id", "123")
    .WithHighPriority()
    .WithSound()
    .WithBadge(1)
    .WithChannelId("promotions")
    .WithWebLink("https://example.com/promo")
    .Build();

var result = await _fcmService.SendAsync(message);
```

### 主題推播

```csharp
// 發送到主題
var result = await _fcmService.SendToTopicAsync("news",
    new Notification { Title = "最新消息", Body = "今日頭條新聞" });

// 條件推播
var result = await _fcmService.SendToConditionAsync(
    "'news' in topics && 'premium' in topics",
    new Notification { Title = "VIP 專屬", Body = "VIP 會員專屬優惠" });
```

### 批次推播

```csharp
var tokens = new List<string> { "token1", "token2", "token3" };

var batchResult = await _fcmService.SendMulticastAsync(tokens,
    new Notification { Title = "批次通知", Body = "群發訊息" });

Console.WriteLine($"成功: {batchResult.SuccessCount}, 失敗: {batchResult.FailureCount}");

// 檢查失效的 Token
if (batchResult.HasUnregisteredTokens)
{
    Console.WriteLine($"已失效的 Token: {string.Join(", ", batchResult.UnregisteredTokens)}");
}
```

### 排程推播

```csharp
// 延遲發送
var result = await _fcmService.SendScheduledAsync("device_token",
    new Notification { Title = "提醒", Body = "別忘了！" },
    TimeSpan.FromMinutes(30));

// 指定時間發送
var result = await _fcmService.SendScheduledAsync("device_token",
    new Notification { Title = "排程通知", Body = "定時推播" },
    new DateTimeOffset(2025, 12, 25, 9, 0, 0, TimeSpan.FromHours(8)));
```

### 主題訂閱 / 取消訂閱

```csharp
// 訂閱主題
var result = await _fcmService.SubscribeToTopicAsync("news", "device_token");

// 批次訂閱
var result = await _fcmService.SubscribeToTopicAsync("news",
    new List<string> { "token1", "token2" });

// 取消訂閱
var result = await _fcmService.UnsubscribeFromTopicAsync("news", "device_token");
```

### 驗證訊息格式（不實際發送）

```csharp
var message = new FCMMessage
{
    Token = "device_token",
    Notification = new Notification { Title = "測試", Body = "驗證格式" }
};

var result = await _fcmService.ValidateAsync(message);
```

## 可用方法總覽

### 單一裝置

| 方法 | 說明 |
|------|------|
| `SendAsync(token, notification)` | 發送通知到單一裝置 |
| `SendAsync(token, notification, data)` | 發送通知 + 自訂資料到單一裝置 |
| `SendDataAsync(token, data)` | 發送純資料靜默推播 |
| `SendAsync(FCMMessage)` | 發送完全自訂訊息 |
| `ValidateAsync(FCMMessage)` | 驗證訊息格式（不發送） |

### 主題

| 方法 | 說明 |
|------|------|
| `SendToTopicAsync(topic, notification)` | 發送通知到主題 |
| `SendToTopicAsync(topic, notification, data)` | 發送通知 + 資料到主題 |
| `SendDataToTopicAsync(topic, data)` | 發送純資料到主題 |

### 條件

| 方法 | 說明 |
|------|------|
| `SendToConditionAsync(condition, notification)` | 條件推播 |
| `SendToConditionAsync(condition, notification, data)` | 條件推播 + 資料 |

### 批次 / 多播

| 方法 | 說明 |
|------|------|
| `SendMulticastAsync(tokens, notification)` | 批次推播到多個裝置 |
| `SendMulticastAsync(tokens, notification, data)` | 批次推播 + 資料 |
| `SendBatchAsync(messages)` | 批次發送多個不同訊息 |

### 排程

| 方法 | 說明 |
|------|------|
| `SendScheduledAsync(token, notification, delay)` | 延遲 TimeSpan 後發送 |
| `SendScheduledAsync(token, notification, data, delay)` | 附帶資料延遲發送 |
| `SendScheduledAsync(token, notification, sendAt)` | 指定 DateTimeOffset 時間發送 |
| `SendScheduledAsync(message, delay)` | 自訂訊息延遲發送 |
| `SendScheduledAsync(message, sendAt)` | 自訂訊息指定時間發送 |

### 主題管理

| 方法 | 說明 |
|------|------|
| `SubscribeToTopicAsync(topic, token)` | 訂閱主題（單一裝置） |
| `SubscribeToTopicAsync(topic, tokens)` | 訂閱主題（多裝置） |
| `UnsubscribeFromTopicAsync(topic, token)` | 取消訂閱（單一裝置） |
| `UnsubscribeFromTopicAsync(topic, tokens)` | 取消訂閱（多裝置） |

## 錯誤處理

套件提供分類的例外類別：

| 例外類別 | 說明 |
|----------|------|
| `FCMException` | 所有 FCM 錯誤的基底例外 |
| `FCMConfigurationException` | 設定錯誤（缺少 ProjectId、無效金鑰） |
| `FCMAuthenticationException` | 認證錯誤（無效憑證、Token 取得失敗） |

```csharp
try
{
    var result = await _fcmService.SendAsync("token",
        new Notification { Title = "測試", Body = "Hello" });
}
catch (FCMConfigurationException ex)
{
    // 處理設定錯誤
}
catch (FCMAuthenticationException ex)
{
    // 處理認證錯誤
}
catch (FCMException ex)
{
    // 處理其他 FCM 錯誤
    Console.WriteLine($"StatusCode: {ex.StatusCode}, ErrorCode: {ex.ErrorCode}");
}
```

## Firebase 專案建立

前往 https://console.firebase.google.com/ 建立你的專案。

## 授權條款

MIT
