using Belem.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
namespace Belem.Core
{
    public static class Startup
    {
        public static IServiceCollection ConfigureDependencies(this IServiceCollection services , IConfiguration configuration) 
        {

            var appSettings = new AppSettings();
            var appConfig = configuration.GetSection("AppSettings");
            appConfig.Bind(appSettings);

            services.AddSingleton(appSettings);
            services.AddSingleton(new TelegramBotClient(appSettings.BotApiKey));
            services.AddSingleton<TelegramService>();
            services.AddSingleton<ImageProcessor>();
            services.AddScoped<TradingService>();
            return services;
        }
    }
}
