namespace Ozakboy.FCM.Models.Responses
{
    /// <summary>
    /// 批次發送回應
    /// </summary>
    public class BatchResponse
    {
        /// <summary>
        /// 各筆發送結果
        /// </summary>
        public List<SendResult> Results { get; set; } = new();

        /// <summary>
        /// 成功數量
        /// </summary>
        public int SuccessCount => Results.Count(r => r.IsSuccess);

        /// <summary>
        /// 失敗數量
        /// </summary>
        public int FailureCount => Results.Count(r => !r.IsSuccess);

        /// <summary>
        /// 總數量
        /// </summary>
        public int TotalCount => Results.Count;

        /// <summary>
        /// 是否全部成功
        /// </summary>
        public bool IsAllSuccess => FailureCount == 0;
    }

    /// <summary>
    /// 個別發送結果
    /// </summary>
    public class SendResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 目標 Token
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// 訊息名稱（成功時回傳）
        /// </summary>
        public string? MessageName { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// 錯誤代碼
        /// </summary>
        public string? ErrorCode { get; set; }
    }
}
