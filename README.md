[![nuget](https://img.shields.io/badge/nuget-ozakboy.FCM-blue)](https://www.nuget.org/packages/Ozakboy.FCM/) [![github](https://img.shields.io/badge/github-ozakboy.FCM-blue)](https://github.com/ozakboy/ozakboy.FCM/)

## Language / 語言

- [繁體中文](wiki/README.zh-TW.md)

## Requirements

- .NET 6.0 / .NET 7.0 / .NET 8.0 / .NET 9.0

## Features

A complete Google Firebase Cloud Messaging (FCM) HTTP v1 API integration library for .NET:

- Single device push notifications
- Topic-based push notifications
- Condition-based push notifications
- Batch / Multicast push notifications (auto-chunking for 500+ recipients)
- Scheduled push notifications (delay or specific time)
- Topic subscribe / unsubscribe management
- Data-only silent push notifications
- Message validation (dry-run without sending)
- Platform-specific configuration for Android / iOS (APNs) / Web Push
- Fluent message builder API
- OAuth2 Service Account authentication with automatic token caching & refresh
- Exponential backoff retry with Retry-After header support
- Full logging via ozakboy.NLOG

## Installation

```
dotnet add package Ozakboy.FCM
```

## Configuration

### 1. Obtain Firebase Service Account Key

Go to [Firebase Console](https://console.firebase.google.com/) -> Project Settings -> Service Accounts -> Generate New Private Key

### 2. Configure appsettings.json

**Option A: Specify key file path**
```json
{
  "FCMSettings": {
    "ProjectId": "your-firebase-project-id",
    "ServiceAccountKeyPath": "path/to/serviceAccountKey.json"
  }
}
```

**Option B: Embed key content directly**
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

**Retry Settings (optional)**
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

### 3. Register Services (Program.cs)

```csharp
using Ozakboy.FCM.Extensions;

// Option A: Load from Configuration
builder.Services.AddOzakboyFCM(builder.Configuration);

// Option B: Configure in code
builder.Services.AddOzakboyFCM(options =>
{
    options.ProjectId = "your-firebase-project-id";
    options.ServiceAccountKeyPath = "serviceAccountKey.json";
});
```

## Usage

### Inject the Service

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

### Send Notification to a Single Device

```csharp
var result = await _fcmService.SendAsync("device_token",
    new Notification { Title = "Hello", Body = "World" });

if (result.IsSuccess)
    Console.WriteLine($"Success: {result.MessageName}");
else
    Console.WriteLine($"Failed: {result.ErrorMessage}");
```

### Send Notification with Custom Data

```csharp
var result = await _fcmService.SendAsync("device_token",
    new Notification { Title = "Order Update", Body = "Your order has been shipped" },
    new Dictionary<string, string>
    {
        { "order_id", "12345" },
        { "action", "open_order" }
    });
```

### Send Silent Push (Data-only)

```csharp
var result = await _fcmService.SendDataAsync("device_token",
    new Dictionary<string, string>
    {
        { "sync", "true" },
        { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() }
    });
```

### Send Fully Custom Message with Platform-Specific Settings

```csharp
var message = new FCMMessage
{
    Token = "device_token",
    Notification = new Notification
    {
        Title = "Promotion",
        Body = "Limited time 20% off!",
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

### Using the Fluent Message Builder

```csharp
using Ozakboy.FCM.Builders;

var message = FCMMessageBuilder.Create()
    .ToToken("device_token")
    .WithNotification("Promotion", "Limited time offer!")
    .AddData("promo_id", "123")
    .WithHighPriority()
    .WithSound()
    .WithBadge(1)
    .WithChannelId("promotions")
    .WithWebLink("https://example.com/promo")
    .Build();

var result = await _fcmService.SendAsync(message);
```

### Topic Push

```csharp
// Send to a topic
var result = await _fcmService.SendToTopicAsync("news",
    new Notification { Title = "Breaking News", Body = "Today's headline" });

// Condition-based push
var result = await _fcmService.SendToConditionAsync(
    "'news' in topics && 'premium' in topics",
    new Notification { Title = "VIP Exclusive", Body = "Premium member offer" });
```

### Batch / Multicast Push

```csharp
var tokens = new List<string> { "token1", "token2", "token3" };

var batchResult = await _fcmService.SendMulticastAsync(tokens,
    new Notification { Title = "Batch Notification", Body = "Broadcast message" });

Console.WriteLine($"Success: {batchResult.SuccessCount}, Failed: {batchResult.FailureCount}");

// Check for invalid tokens
if (batchResult.HasUnregisteredTokens)
{
    Console.WriteLine($"Unregistered tokens: {string.Join(", ", batchResult.UnregisteredTokens)}");
}
```

### Scheduled Push

```csharp
// Send after a delay
var result = await _fcmService.SendScheduledAsync("device_token",
    new Notification { Title = "Reminder", Body = "Don't forget!" },
    TimeSpan.FromMinutes(30));

// Send at a specific time
var result = await _fcmService.SendScheduledAsync("device_token",
    new Notification { Title = "Scheduled", Body = "Timed notification" },
    new DateTimeOffset(2025, 12, 25, 9, 0, 0, TimeSpan.FromHours(8)));
```

### Topic Subscribe / Unsubscribe

```csharp
// Subscribe to a topic
var result = await _fcmService.SubscribeToTopicAsync("news", "device_token");

// Batch subscribe
var result = await _fcmService.SubscribeToTopicAsync("news",
    new List<string> { "token1", "token2" });

// Unsubscribe
var result = await _fcmService.UnsubscribeFromTopicAsync("news", "device_token");
```

### Validate Message Format (Dry Run)

```csharp
var message = new FCMMessage
{
    Token = "device_token",
    Notification = new Notification { Title = "Test", Body = "Validate format" }
};

var result = await _fcmService.ValidateAsync(message);
```

## API Reference

### Single Device

| Method | Description |
|--------|-------------|
| `SendAsync(token, notification)` | Send notification to a single device |
| `SendAsync(token, notification, data)` | Send notification + custom data |
| `SendDataAsync(token, data)` | Send data-only silent push |
| `SendAsync(FCMMessage)` | Send fully custom message |
| `ValidateAsync(FCMMessage)` | Validate message format (dry run) |

### Topic

| Method | Description |
|--------|-------------|
| `SendToTopicAsync(topic, notification)` | Send notification to a topic |
| `SendToTopicAsync(topic, notification, data)` | Send notification + data to a topic |
| `SendDataToTopicAsync(topic, data)` | Send data-only to a topic |

### Condition

| Method | Description |
|--------|-------------|
| `SendToConditionAsync(condition, notification)` | Condition-based push |
| `SendToConditionAsync(condition, notification, data)` | Condition-based push + data |

### Batch / Multicast

| Method | Description |
|--------|-------------|
| `SendMulticastAsync(tokens, notification)` | Batch push to multiple devices |
| `SendMulticastAsync(tokens, notification, data)` | Batch push + data |
| `SendBatchAsync(messages)` | Send multiple different messages |

### Scheduled

| Method | Description |
|--------|-------------|
| `SendScheduledAsync(token, notification, delay)` | Send after TimeSpan delay |
| `SendScheduledAsync(token, notification, data, delay)` | Send with data after delay |
| `SendScheduledAsync(token, notification, sendAt)` | Send at specific DateTimeOffset |
| `SendScheduledAsync(message, delay)` | Send custom message after delay |
| `SendScheduledAsync(message, sendAt)` | Send custom message at specific time |

### Topic Management

| Method | Description |
|--------|-------------|
| `SubscribeToTopicAsync(topic, token)` | Subscribe single device to topic |
| `SubscribeToTopicAsync(topic, tokens)` | Subscribe multiple devices to topic |
| `UnsubscribeFromTopicAsync(topic, token)` | Unsubscribe single device |
| `UnsubscribeFromTopicAsync(topic, tokens)` | Unsubscribe multiple devices |

## Error Handling

The library provides typed exceptions:

| Exception | Description |
|-----------|-------------|
| `FCMException` | Base exception for all FCM errors |
| `FCMConfigurationException` | Configuration errors (missing ProjectId, invalid key) |
| `FCMAuthenticationException` | Authentication errors (invalid credentials, token failure) |

```csharp
try
{
    var result = await _fcmService.SendAsync("token",
        new Notification { Title = "Test", Body = "Hello" });
}
catch (FCMConfigurationException ex)
{
    // Handle configuration errors
}
catch (FCMAuthenticationException ex)
{
    // Handle authentication errors
}
catch (FCMException ex)
{
    // Handle other FCM errors
    Console.WriteLine($"StatusCode: {ex.StatusCode}, ErrorCode: {ex.ErrorCode}");
}
```

## Firebase Project Setup

Go to https://console.firebase.google.com/ to create your project.

## License

MIT
