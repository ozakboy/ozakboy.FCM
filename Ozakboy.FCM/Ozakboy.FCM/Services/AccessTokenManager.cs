using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Ozakboy.FCM.Exceptions;
using Ozakboy.FCM.Models.Settings;
using ozakboy.NLOG;

namespace Ozakboy.FCM.Services
{
    /// <summary>
    /// OAuth2 Access Token 管理器
    /// 負責取得、快取及自動刷新 Google Service Account 的 Access Token
    /// </summary>
    internal class AccessTokenManager
    {
        private readonly FCMSettings _settings;
        private readonly IMemoryCache _cache;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        private const string CacheKey = "FCM_AccessToken";
        private static readonly string[] Scopes = new[] { "https://www.googleapis.com/auth/firebase.messaging" };

        public AccessTokenManager(IOptions<FCMSettings> settings, IMemoryCache cache)
        {
            _settings = settings.Value;
            _cache = cache;
        }

        /// <summary>
        /// 取得有效的 Access Token（自動快取及刷新）
        /// </summary>
        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(CacheKey, out string? cachedToken) && !string.IsNullOrEmpty(cachedToken))
            {
                return cachedToken;
            }

            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                // Double-check after acquiring lock
                if (_cache.TryGetValue(CacheKey, out cachedToken) && !string.IsNullOrEmpty(cachedToken))
                {
                    return cachedToken;
                }

                LOG.Debug_Log("正在取得 FCM Access Token...");

                GoogleCredential credential = CreateCredential();
                var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync(
                    cancellationToken: cancellationToken);

                if (string.IsNullOrEmpty(token))
                {
                    throw new FCMAuthenticationException("無法取得 Access Token，回傳值為空");
                }

                // 快取 Token，設定 50 分鐘到期（Google Token 通常 60 分鐘有效）
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(50));
                _cache.Set(CacheKey, token, cacheOptions);

                LOG.Info_Log("FCM Access Token 取得成功，已快取 50 分鐘");

                return token;
            }
            catch (FCMException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LOG.Error_Log("取得 FCM Access Token 失敗", ex);
                throw new FCMAuthenticationException("取得 FCM Access Token 失敗", ex);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// 清除快取的 Token（強制下次重新取得）
        /// </summary>
        public void InvalidateToken()
        {
            _cache.Remove(CacheKey);
            LOG.Debug_Log("FCM Access Token 快取已清除");
        }

        private GoogleCredential CreateCredential()
        {
            GoogleCredential credential;

            if (!string.IsNullOrEmpty(_settings.ServiceAccountKeyPath))
            {
                // 從檔案路徑讀取
                if (!File.Exists(_settings.ServiceAccountKeyPath))
                {
                    throw new FCMConfigurationException(
                        $"Service Account 金鑰檔案不存在: {_settings.ServiceAccountKeyPath}");
                }

                LOG.Debug_Log($"從檔案載入 Service Account 金鑰: {_settings.ServiceAccountKeyPath}");
                using var stream = File.OpenRead(_settings.ServiceAccountKeyPath);
                credential = GoogleCredential.FromStream(stream);
            }
            else if (_settings.ServiceAccountKey != null &&
                     !string.IsNullOrEmpty(_settings.ServiceAccountKey.private_key))
            {
                // 從設定中的 JSON 內容讀取
                LOG.Debug_Log("從設定內容載入 Service Account 金鑰");
                var json = System.Text.Json.JsonSerializer.Serialize(_settings.ServiceAccountKey);
                credential = GoogleCredential.FromJson(json);
            }
            else
            {
                throw new FCMConfigurationException(
                    "請設定 ServiceAccountKeyPath 或 ServiceAccountKey，" +
                    "至少需提供一種 Service Account 認證方式");
            }

            return credential.CreateScoped(Scopes);
        }
    }
}
