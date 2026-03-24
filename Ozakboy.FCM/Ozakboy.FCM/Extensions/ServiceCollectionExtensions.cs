using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozakboy.FCM.Interfaces;
using Ozakboy.FCM.Models.Settings;
using Ozakboy.FCM.Services;

namespace Ozakboy.FCM.Extensions
{
    /// <summary>
    /// DI 容器擴充方法
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 註冊 FCM 推播通知服務（從 IConfiguration 讀取設定）
        /// 設定區段名稱為 "FCMSettings"
        /// </summary>
        /// <param name="services">服務集合</param>
        /// <param name="configuration">設定來源</param>
        /// <returns>服務集合</returns>
        public static IServiceCollection AddOzakboyFCM(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FCMSettings>(configuration.GetSection(FCMSettings.SectionName));
            RegisterServices(services);
            return services;
        }

        /// <summary>
        /// 註冊 FCM 推播通知服務（使用 Action 設定）
        /// </summary>
        /// <param name="services">服務集合</param>
        /// <param name="configure">設定委派</param>
        /// <returns>服務集合</returns>
        public static IServiceCollection AddOzakboyFCM(this IServiceCollection services, Action<FCMSettings> configure)
        {
            services.Configure(configure);
            RegisterServices(services);
            return services;
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddMemoryCache();
            services.AddSingleton<AccessTokenManager>();
            services.AddScoped<IFCMService, FCMService>();
        }
    }
}
