using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyNChat.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEasyNChat(this IServiceCollection services)
        {
            services.AddSingleton<LogService>();
            services.AddSingleton<SendMessageService>();
            services.AddSingleton<IHostedService, InitService>();
            services.AddSingleton<WebSocketService>();

            return services;
        }

        public static IApplicationBuilder UseWsEasyNChat(this IApplicationBuilder builder)
        {
            using (var serviceScope = builder.ApplicationServices.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var myDependency = services.GetRequiredService<WebSocketService>();
                myDependency.StartService();
            }
            return builder;
        }
    }
}
