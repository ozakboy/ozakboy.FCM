using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Ozakboy.FCM.Exceptions;
using Ozakboy.FCM.Interfaces;
using Ozakboy.FCM.Models.Messages;
using Ozakboy.FCM.Models.Responses;
using Ozakboy.FCM.Models.Settings;
using ozakboy.NLOG;

namespace Ozakboy.FCM.Services
{
    /// <summary>
    /// FCM 推播通知服務實作
    /// 使用 Google Firebase Cloud Messaging HTTP v1 API
    /// </summary>
    public class FCMService : IFCMService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AccessTokenManager _tokenManager;
        private readonly FCMSettings _settings;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// FCM v1 API 端點
        /// </summary>
        private string SendEndpoint => $"https://fcm.googleapis.com/v1/projects/{_settings.ProjectId}/messages:send";

        /// <summary>
        /// IID (Instance ID) API 端點 - 用於主題管理
        /// </summary>
        private const string TopicSubscribeEndpoint = "https://iid.googleapis.com/iid/v1:batchAdd";
        private const string TopicUnsubscribeEndpoint = "https://iid.googleapis.com/iid/v1:batchRemove";

        public FCMService(
            IHttpClientFactory httpClientFactory,
            AccessTokenManager tokenManager,
            IOptions<FCMSettings> settings)
        {
            _httpClientFactory = httpClientFactory;
            _tokenManager = tokenManager;
            _settings = settings.Value;

            ValidateSettings();
        }

        private void ValidateSettings()
        {
            if (string.IsNullOrEmpty(_settings.ProjectId))
            {
                throw new FCMConfigurationException("FCMSettings.ProjectId 不可為空，請在設定中填入 Firebase 專案 ID");
            }
        }

        #region 單一裝置推播

        /// <inheritdoc/>
        public Task<FCMResponse> SendAsync(string token, Notification notification, CancellationToken cancellationToken = default)
        {
            ValidateToken(token);
            var message = new FCMMessage { Token = token, Notification = notification };
            return SendInternalAsync(message, false, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<FCMResponse> SendAsync(string token, Notification notification, Dictionary<string, string> data, CancellationToken cancellationToken = default)
        {
            ValidateToken(token);
            var message = new FCMMessage { Token = token, Notification = notification, Data = data };
            return SendInternalAsync(message, false, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<FCMResponse> SendDataAsync(string token, Dictionary<string, string> data, CancellationToken cancellationToken = default)
        {
            ValidateToken(token);
            if (data == null || data.Count == 0)
                throw new ArgumentException("Data 不可為空", nameof(data));

            var message = new FCMMessage { Token = token, Data = data };
            return SendInternalAsync(message, false, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<FCMResponse> SendAsync(FCMMessage message, CancellationToken cancellationToken = default)
        {
            ValidateMessage(message);
            return SendInternalAsync(message, false, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<FCMResponse> ValidateAsync(FCMMessage message, CancellationToken cancellationToken = default)
        {
            ValidateMessage(message);
            return SendInternalAsync(message, true, cancellationToken);
        }

        #endregion

        #region 主題推播

        /// <inheritdoc/>
        public Task<FCMResponse> SendToTopicAsync(string topic, Notification notification, CancellationToken cancellationToken = default)
        {
            ValidateTopic(topic);
            var message = new FCMMessage { Topic = topic, Notification = notification };
            return SendInternalAsync(message, false, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<FCMResponse> SendToTopicAsync(string topic, Notification notification, Dictionary<string, string> data, CancellationToken cancellationToken = default)
        {
            ValidateTopic(topic);
            var message = new FCMMessage { Topic = topic, Notification = notification, Data = data };
            return SendInternalAsync(message, false, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<FCMResponse> SendDataToTopicAsync(string topic, Dictionary<string, string> data, CancellationToken cancellationToken = default)
        {
            ValidateTopic(topic);
            if (data == null || data.Count == 0)
                throw new ArgumentException("Data 不可為空", nameof(data));

            var message = new FCMMessage { Topic = topic, Data = data };
            return SendInternalAsync(message, false, cancellationToken);
        }

        #endregion

        #region 條件推播

        /// <inheritdoc/>
        public Task<FCMResponse> SendToConditionAsync(string condition, Notification notification, CancellationToken cancellationToken = default)
        {
            ValidateCondition(condition);
            var message = new FCMMessage { Condition = condition, Notification = notification };
            return SendInternalAsync(message, false, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<FCMResponse> SendToConditionAsync(string condition, Notification notification, Dictionary<string, string> data, CancellationToken cancellationToken = default)
        {
            ValidateCondition(condition);
            var message = new FCMMessage { Condition = condition, Notification = notification, Data = data };
            return SendInternalAsync(message, false, cancellationToken);
        }

        #endregion

        #region 批次推播

        /// <inheritdoc/>
        public Task<BatchResponse> SendMulticastAsync(List<string> tokens, Notification notification, CancellationToken cancellationToken = default)
        {
            ValidateTokenList(tokens);
            var messages = tokens.Select(t => new FCMMessage { Token = t, Notification = notification }).ToList();
            return SendBatchInternalAsync(messages, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<BatchResponse> SendMulticastAsync(List<string> tokens, Notification notification, Dictionary<string, string> data, CancellationToken cancellationToken = default)
        {
            ValidateTokenList(tokens);
            var messages = tokens.Select(t => new FCMMessage { Token = t, Notification = notification, Data = data }).ToList();
            return SendBatchInternalAsync(messages, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<BatchResponse> SendBatchAsync(List<FCMMessage> messages, CancellationToken cancellationToken = default)
        {
            if (messages == null || messages.Count == 0)
                throw new ArgumentException("訊息列表不可為空", nameof(messages));
            if (messages.Count > 500)
                throw new ArgumentException("批次發送最多 500 筆訊息", nameof(messages));

            return SendBatchInternalAsync(messages, cancellationToken);
        }

        #endregion

        #region 主題管理

        /// <inheritdoc/>
        public Task<TopicResponse> SubscribeToTopicAsync(string topic, List<string> tokens, CancellationToken cancellationToken = default)
        {
            ValidateTopic(topic);
            ValidateTokenList(tokens);
            return TopicManagementAsync(TopicSubscribeEndpoint, topic, tokens, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TopicResponse> SubscribeToTopicAsync(string topic, string token, CancellationToken cancellationToken = default)
        {
            ValidateTopic(topic);
            ValidateToken(token);
            return TopicManagementAsync(TopicSubscribeEndpoint, topic, new List<string> { token }, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TopicResponse> UnsubscribeFromTopicAsync(string topic, List<string> tokens, CancellationToken cancellationToken = default)
        {
            ValidateTopic(topic);
            ValidateTokenList(tokens);
            return TopicManagementAsync(TopicUnsubscribeEndpoint, topic, tokens, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TopicResponse> UnsubscribeFromTopicAsync(string topic, string token, CancellationToken cancellationToken = default)
        {
            ValidateTopic(topic);
            ValidateToken(token);
            return TopicManagementAsync(TopicUnsubscribeEndpoint, topic, new List<string> { token }, cancellationToken);
        }

        #endregion

        #region 內部方法

        /// <summary>
        /// 核心發送方法
        /// </summary>
        private async Task<FCMResponse> SendInternalAsync(FCMMessage message, bool validateOnly, CancellationToken cancellationToken)
        {
            var request = new FCMSendRequest
            {
                Message = message,
                ValidateOnly = validateOnly ? true : null
            };

            var logPrefix = validateOnly ? "[驗證]" : "[發送]";
            var target = message.Token ?? message.Topic ?? message.Condition ?? "unknown";
            LOG.Info_Log($"{logPrefix} FCM 訊息 -> 目標: {target}");

            try
            {
                var accessToken = await _tokenManager.GetAccessTokenAsync(cancellationToken);
                var client = _httpClientFactory.CreateClient();

                var jsonContent = JsonSerializer.Serialize(request, JsonOptions);
                LOG.Debug_Log($"{logPrefix} 請求內容: {jsonContent}");

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, SendEndpoint)
                {
                    Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
                };
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var httpResponse = await client.SendAsync(httpRequest, cancellationToken);
                var responseBody = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

                LOG.Debug_Log($"{logPrefix} 回應狀態: {(int)httpResponse.StatusCode}, 內容: {responseBody}");

                if (httpResponse.IsSuccessStatusCode)
                {
                    var successResponse = JsonSerializer.Deserialize<FCMResponse>(responseBody, JsonOptions);
                    var result = successResponse ?? new FCMResponse();
                    result.IsSuccess = true;
                    result.StatusCode = (int)httpResponse.StatusCode;
                    result.RawResponse = responseBody;

                    LOG.Info_Log($"{logPrefix} 發送成功 -> MessageName: {result.MessageName}");
                    return result;
                }
                else
                {
                    // 如果是 401，清除 Token 快取
                    if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        _tokenManager.InvalidateToken();
                        LOG.Warn_Log($"{logPrefix} Access Token 已過期，已清除快取");
                    }

                    return ParseErrorResponse(responseBody, (int)httpResponse.StatusCode, logPrefix);
                }
            }
            catch (FCMException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LOG.Error_Log($"{logPrefix} FCM 發送失敗", ex);
                throw new FCMException($"FCM 發送失敗: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 批次發送內部方法
        /// </summary>
        private async Task<BatchResponse> SendBatchInternalAsync(List<FCMMessage> messages, CancellationToken cancellationToken)
        {
            LOG.Info_Log($"[批次發送] 共 {messages.Count} 筆訊息");

            var batchResponse = new BatchResponse();
            var tasks = new List<Task<(FCMMessage message, FCMResponse response)>>();

            // 使用 SemaphoreSlim 限制並行數量，避免瞬間大量請求
            var semaphore = new SemaphoreSlim(10);

            foreach (var message in messages)
            {
                tasks.Add(SendWithSemaphoreAsync(semaphore, message, cancellationToken));
            }

            var results = await Task.WhenAll(tasks);

            foreach (var (message, response) in results)
            {
                batchResponse.Results.Add(new SendResult
                {
                    IsSuccess = response.IsSuccess,
                    Token = message.Token,
                    MessageName = response.MessageName,
                    ErrorMessage = response.ErrorMessage,
                    ErrorCode = response.ErrorCode
                });
            }

            LOG.Info_Log($"[批次發送] 完成 - 成功: {batchResponse.SuccessCount}, 失敗: {batchResponse.FailureCount}");
            return batchResponse;
        }

        private async Task<(FCMMessage message, FCMResponse response)> SendWithSemaphoreAsync(
            SemaphoreSlim semaphore, FCMMessage message, CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var response = await SendInternalAsync(message, false, cancellationToken);
                return (message, response);
            }
            catch (Exception ex)
            {
                return (message, new FCMResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                });
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// 主題管理操作
        /// </summary>
        private async Task<TopicResponse> TopicManagementAsync(
            string endpoint, string topic, List<string> tokens, CancellationToken cancellationToken)
        {
            var action = endpoint.Contains("batchAdd") ? "訂閱" : "取消訂閱";
            LOG.Info_Log($"[主題{action}] 主題: {topic}, Token 數量: {tokens.Count}");

            try
            {
                var accessToken = await _tokenManager.GetAccessTokenAsync(cancellationToken);
                var client = _httpClientFactory.CreateClient();

                var requestBody = new
                {
                    to = $"/topics/{topic}",
                    registration_tokens = tokens
                };

                var jsonContent = JsonSerializer.Serialize(requestBody, JsonOptions);
                LOG.Debug_Log($"[主題{action}] 請求內容: {jsonContent}");

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
                {
                    Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
                };
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var httpResponse = await client.SendAsync(httpRequest, cancellationToken);
                var responseBody = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

                LOG.Debug_Log($"[主題{action}] 回應狀態: {(int)httpResponse.StatusCode}, 內容: {responseBody}");

                if (httpResponse.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<TopicResponse>(responseBody, JsonOptions)
                                 ?? new TopicResponse();

                    result.StatusCode = (int)httpResponse.StatusCode;
                    result.RawResponse = responseBody;

                    // 計算成功/失敗數量
                    if (result.Results != null)
                    {
                        result.SuccessCount = result.Results.Count(r => r == null);
                        result.FailureCount = result.Results.Count(r => r != null);
                    }
                    else
                    {
                        result.SuccessCount = tokens.Count;
                        result.FailureCount = 0;
                    }

                    LOG.Info_Log($"[主題{action}] 完成 - 成功: {result.SuccessCount}, 失敗: {result.FailureCount}");
                    return result;
                }
                else
                {
                    if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        _tokenManager.InvalidateToken();
                    }

                    var errorResult = new TopicResponse
                    {
                        StatusCode = (int)httpResponse.StatusCode,
                        RawResponse = responseBody,
                        ErrorMessage = $"主題{action}失敗，HTTP 狀態碼: {(int)httpResponse.StatusCode}",
                        FailureCount = tokens.Count
                    };

                    LOG.Error_Log($"[主題{action}] 失敗 - HTTP {(int)httpResponse.StatusCode}: {responseBody}");
                    return errorResult;
                }
            }
            catch (FCMException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LOG.Error_Log($"[主題{action}] 失敗", ex);
                throw new FCMException($"主題{action}操作失敗: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 解析錯誤回應
        /// </summary>
        private FCMResponse ParseErrorResponse(string responseBody, int statusCode, string logPrefix)
        {
            var result = new FCMResponse
            {
                IsSuccess = false,
                StatusCode = statusCode,
                RawResponse = responseBody
            };

            try
            {
                var errorResponse = JsonSerializer.Deserialize<FCMErrorResponse>(responseBody, JsonOptions);
                if (errorResponse?.Error != null)
                {
                    result.ErrorMessage = errorResponse.Error.Message;
                    result.ErrorCode = errorResponse.Error.Status;

                    // 嘗試取得更具體的 FCM 錯誤代碼
                    var fcmDetail = errorResponse.Error.Details?.FirstOrDefault(d =>
                        d.Type?.Contains("FcmError") == true);
                    if (fcmDetail?.ErrorCode != null)
                    {
                        result.ErrorCode = fcmDetail.ErrorCode;
                    }
                }
            }
            catch
            {
                result.ErrorMessage = responseBody;
            }

            LOG.Error_Log($"{logPrefix} 發送失敗 -> HTTP {statusCode}, ErrorCode: {result.ErrorCode}, Message: {result.ErrorMessage}");
            return result;
        }

        #endregion

        #region 參數驗證

        private static void ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("裝置 Token 不可為空", nameof(token));
        }

        private static void ValidateTopic(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentException("主題名稱不可為空", nameof(topic));
        }

        private static void ValidateCondition(string condition)
        {
            if (string.IsNullOrWhiteSpace(condition))
                throw new ArgumentException("條件運算式不可為空", nameof(condition));
        }

        private static void ValidateTokenList(List<string> tokens)
        {
            if (tokens == null || tokens.Count == 0)
                throw new ArgumentException("Token 列表不可為空", nameof(tokens));
            if (tokens.Count > 1000)
                throw new ArgumentException("Token 列表最多 1000 個", nameof(tokens));
            if (tokens.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Token 列表中不可包含空值", nameof(tokens));
        }

        private static void ValidateMessage(FCMMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var targetCount = 0;
            if (!string.IsNullOrEmpty(message.Token)) targetCount++;
            if (!string.IsNullOrEmpty(message.Topic)) targetCount++;
            if (!string.IsNullOrEmpty(message.Condition)) targetCount++;

            if (targetCount == 0)
                throw new ArgumentException("訊息必須指定一個目標（Token、Topic 或 Condition）", nameof(message));
            if (targetCount > 1)
                throw new ArgumentException("訊息只能指定一個目標（Token、Topic 或 Condition 擇一）", nameof(message));
        }

        #endregion
    }
}
