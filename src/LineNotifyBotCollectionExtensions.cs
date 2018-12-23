using LexLibrary.Line.NotifyBot.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace LexLibrary.Line.NotifyBot
{
    /// <summary>
    /// IServiceCollection 擴充方法
    /// </summary>
    public static class LineNotifyBotCollectionExtensions
    {
        /// <summary>
        /// 將 LineNotifyBot 加入至專案
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IServiceCollection AddLineNotifyBot(this IServiceCollection services, LineNotifyBotSetting setting)
        {
            services.AddScoped((sp) =>
            {
                var httpClientFactory = sp.GetService<IHttpClientFactory>();
                var logger = sp.GetService<ILogger<LineNotifyBotApi>>();

                return new LineNotifyBotApi(setting, httpClientFactory, logger);
            });

            return services;
        }

        /// <summary>
        /// 將 LineNotifyBot 加入至專案
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddLineNotifyBot(this IServiceCollection services, Func<IServiceProvider, LineNotifyBotSetting> setupAction)
        {
            services.AddScoped((sp) =>
            {
                var httpClientFactory = sp.GetService<IHttpClientFactory>();
                var logger = sp.GetService<ILogger<LineNotifyBotApi>>();

                return new LineNotifyBotApi(setupAction(sp), httpClientFactory, logger);
            });

            return services;
        }
    }
}
