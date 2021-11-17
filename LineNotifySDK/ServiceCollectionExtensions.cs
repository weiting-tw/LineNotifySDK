using System;
using FluentValidation;
using LineNotifySDK.Model;
using LineNotifySDK.Validator;
using Microsoft.Extensions.DependencyInjection;

namespace LineNotifySDK
{
    public static class ServiceCollectionExtensions
    {
        public static void AddLineNotifyServices(this IServiceCollection services,
            Action<IServiceProvider, LineNotifyOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddScoped<IValidator<LineNotifyMessage>, LineNotifyMessageValidator>();
            services.AddOptions<LineNotifyOptions>()
                .Configure<IServiceProvider>((options, resolver) => configureOptions(resolver, options));
            services.AddHttpClient("notifyApiClient",
                    x => { x.BaseAddress = new Uri("https://notify-api.line.me"); })
                .ConfigurePrimaryHttpMessageHandler(_ => new HttpLoggingHandler());
            services.AddHttpClient("notifyBotClient",
                    x => { x.BaseAddress = new Uri("https://notify-bot.line.me"); })
                .ConfigurePrimaryHttpMessageHandler(_ => new HttpLoggingHandler());
            services.AddSingleton<ILineNotifyServices, LineNotifyServices>();
        }
    }
}