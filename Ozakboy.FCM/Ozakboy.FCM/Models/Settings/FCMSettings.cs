namespace Ozakboy.FCM.Models.Settings
{
    /// <summary>
    /// FCM 設定模型
    /// </summary>
    public class FCMSettings
    {
        /// <summary>
        /// 設定區段名稱
        /// </summary>
        public const string SectionName = "FCMSettings";

        /// <summary>
        /// Firebase 專案 ID
        /// </summary>
        public string ProjectId { get; set; } = string.Empty;

        /// <summary>
        /// Service Account JSON 金鑰檔案路徑
        /// </summary>
        public string? ServiceAccountKeyPath { get; set; }

        /// <summary>
        /// Service Account JSON 金鑰內容（直接嵌入設定中，與 ServiceAccountKeyPath 擇一使用）
        /// </summary>
        public ServiceAccountKey? ServiceAccountKey { get; set; }

        /// <summary>
        /// 重試設定
        /// </summary>
        public RetrySettings Retry { get; set; } = new();
    }

    /// <summary>
    /// 重試機制設定
    /// </summary>
    public class RetrySettings
    {
        /// <summary>
        /// 是否啟用自動重試（預設啟用）
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 最大重試次數（預設 3 次）
        /// </summary>
        public int MaxRetryCount { get; set; } = 3;

        /// <summary>
        /// 初始重試延遲（毫秒，預設 1000ms）
        /// 後續每次重試延遲加倍（指數退避）
        /// </summary>
        public int InitialDelayMs { get; set; } = 1000;

        /// <summary>
        /// 最大重試延遲（毫秒，預設 30000ms = 30 秒）
        /// </summary>
        public int MaxDelayMs { get; set; } = 30000;
    }

    /// <summary>
    /// Google Service Account 金鑰模型（對應 JSON 金鑰檔案內容）
    /// </summary>
    public class ServiceAccountKey
    {
        public string type { get; set; } = "service_account";
        public string project_id { get; set; } = string.Empty;
        public string private_key_id { get; set; } = string.Empty;
        public string private_key { get; set; } = string.Empty;
        public string client_email { get; set; } = string.Empty;
        public string client_id { get; set; } = string.Empty;
        public string auth_uri { get; set; } = string.Empty;
        public string token_uri { get; set; } = string.Empty;
        public string auth_provider_x509_cert_url { get; set; } = string.Empty;
        public string client_x509_cert_url { get; set; } = string.Empty;
    }
}
