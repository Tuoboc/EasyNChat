using EasyNChat.Interfaces;
using EasyNChat.Models;
using EasyNChat.Services;
using EasyNChat.WebSocket;
using Microsoft.Extensions.Hosting;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyNChat.Services
{
    public class WebSocketService<T> : IHostedService where T : WebSocketSession, new()
    {
        private LogService log;
        private WebSocketManager<T> server;
        public bool IsRunning { get; set; } = false;
        public WebSocketService(LogService logService)
        {
            log = logService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            int errortimes = 0;
            while (GlobalInfo.NodeInfo == null)
            {
                await Task.Delay(1000);
                if (errortimes++ > 60)
                {
                    log.LogError("60 secoends");
                    return;
                }
                   
            }
            if (GlobalInfo.NodeInfo.IsRunning)
            {
                server = new WebSocketManager<T>();
                string path = AppDomain.CurrentDomain.BaseDirectory + "ws";
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                server.AddStaticContent(path, "/wschat");
                server.Start();
                log.LogInformation("WebSocket Chat Service Is Running.");
                IsRunning = true;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            IsRunning = false;
            GlobalInfo.NodeInfo.Redis.KeyDelete("EasyNChat_" + GlobalInfo.NodeInfo.NodeName + "_User");
            log.LogInformation("WebSocket Chat Service Is Stoped.");
            return Task.CompletedTask;
        }
    }
}
