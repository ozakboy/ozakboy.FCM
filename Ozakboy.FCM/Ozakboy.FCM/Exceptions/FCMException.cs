namespace Ozakboy.FCM.Exceptions
{
    /// <summary>
    /// FCM 操作例外
    /// </summary>
    public class FCMException : Exception
    {
        /// <summary>
        /// HTTP 狀態碼
        /// </summary>
        public int? StatusCode { get; }

        /// <summary>
        /// FCM 錯誤代碼
        /// </summary>
        public string? ErrorCode { get; }

        public FCMException(string message)
            : base(message) { }

        public FCMException(string message, Exception innerException)
            : base(message, innerException) { }

        public FCMException(string message, int statusCode, string? errorCode = null)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        public FCMException(string message, int statusCode, string? errorCode, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }

    /// <summary>
    /// FCM 設定例外
    /// </summary>
    public class FCMConfigurationException : FCMException
    {
        public FCMConfigurationException(string message)
            : base(message) { }

        public FCMConfigurationException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// FCM 認證例外
    /// </summary>
    public class FCMAuthenticationException : FCMException
    {
        public FCMAuthenticationException(string message)
            : base(message) { }

        public FCMAuthenticationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
