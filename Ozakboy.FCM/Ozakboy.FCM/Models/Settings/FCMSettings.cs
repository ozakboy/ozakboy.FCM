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
