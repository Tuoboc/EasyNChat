using EasyNChat.Interfaces;
using EasyNChat.Models;
using EasyNChat.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;

namespace EasyNChat.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEasyNWsChat(this IServiceCollection services)
        {
            var logservice = services.Count(a => a.ServiceType == typeof(LogService));
            if (logservice == 0)
                services.AddSingleton<LogService>();
            var mainservice = services.Count(a => a.ServiceType == typeof(EasyNChatService<WebSocketSession>));
            if (mainservice == 0)
                services.AddSingleton<EasyNChatService<WebSocketSession>>();
            services.AddSingleton<WebSocketService<WebSocketSession>>();
            return services;
        }
        public static IServiceCollection AddEasyNWsChat<T>(this IServiceCollection services) where T : WebSocketSession, new()
        {
            var logservice = services.Count(a => a.ServiceType == typeof(LogService));
            if (logservice == 0)
                services.AddSingleton<LogService>();
            var mainservice = services.Count(a => a.ServiceType == typeof(EasyNChatService<T>));
            if (mainservice == 0)
                services.AddSingleton<EasyNChatService<T>>();
            services.AddSingleton<WebSocketService<T>>();
            return services;
        }
        public static IApplicationBuilder UseEasyNWsChat(this IApplicationBuilder builder)
        {
            var service = builder.ApplicationServices.GetService<EasyNChatService<WebSocketSession>>();
            service.StartAsync(new CancellationToken());
            var service2 = builder.ApplicationServices.GetService<WebSocketService<WebSocketSession>>();
            service2.StartAsync(new CancellationToken());
            return builder;
        }
        public static IApplicationBuilder UseEasyNWsChat<T>(this IApplicationBuilder builder) where T : WebSocketSession, new()
        {
            var service = builder.ApplicationServices.GetService<EasyNChatService<T>>();
            service.StartAsync(new CancellationToken());
            var service2 = builder.ApplicationServices.GetService<WebSocketService<T>>();
            service2.StartAsync(new CancellationToken());
            return builder;
        }
    }
}
