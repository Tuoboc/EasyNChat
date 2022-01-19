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

namespace EasyNChat.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEasyNChat(this IServiceCollection services, Action<WsEasyNChatConfig> nodeinfo)
        {
            WsEasyNChatConfig config = new WsEasyNChatConfig();
            nodeinfo(config);
            services.AddSingleton<LogService>();
            services.AddSingleton<IHostedService, EasyNChatService>(sp => new EasyNChatService(config));
            return services;
        }
        public static IServiceCollection AddEasyNChat(this IServiceCollection services)
        {
            services.AddSingleton<LogService>();
            services.AddSingleton<IHostedService, EasyNChatService>();
            return services;
        }
        public static IServiceCollection AddEasyNWsChat(this IServiceCollection services)
        {
            services.AddSingleton<IHostedService, WebSocketService<WebSocketSession>>();
            return services;
        }
        public static IServiceCollection AddEasyNWsChat<T>(this IServiceCollection services) where T : WebSocketSession, new()
        {
            services.AddSingleton<IHostedService, WebSocketService<T>>();
            return services;
        }
    }
}
