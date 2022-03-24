[![nuget](https://img.shields.io/badge/nuget-ozakboy.FCM-blue)](https://www.nuget.org/packages/Ozakboy.FCM/)

## 要求

ASP.net Core 6

## 功能

整合 Firebase Cloud Messaging  發送推播、訂閱主題、主題發送推播 功能


## 部屬

要建立 appsettings.json  檔案在根目錄

```
 "ApplicationID": "雲端通訊 伺服器金鑰",
 "SenderID": "雲端通訊 寄件者 ID",
```

並且在 Program.cs 檔案內註冊元件

```
builder.Services.AddHttpClient();
builder.Services.AddScoped<IFCM, FCM>();
```

## 可以使用的法方

* SubscribeTopic
* FcmSend
* FcmSendTopic

## Firebase 專案建立

前往 https://console.firebase.google.com/u/0/  建立妳的專案
