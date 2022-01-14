using EasyNChat.Interfaces;
using EasyNChat.Models;
using EasyNChat.Services;
using EasyNChat.WebSocket;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EasyNChat.Services
{
    public class WebSocketService : IChatConnection
    {
        private LogService log;
        private WebSocketManager server;
        public bool IsRunning = false;
        public WebSocketService(LogService logService)
        {
            log = logService;
        }

        public async void StartService()
        {
            int errortimes = 0;
            while (GlobalInfo.NodeInfo == null)
            {
                await Task.Delay(1000);
                if (errortimes++ > 60)
                    return;
            }
            if (GlobalInfo.NodeInfo.IsRunning)
            {
                server = new WebSocketManager(IPAddress.Any, GlobalInfo.NodeInfo.WsPort);
                string www = "../../../../../www/ws";
                server.AddStaticContent(www, "/wschat");
                server.Start();
                log.LogInformation("WebSocket Chat Service Is Running.");
                IsRunning = true;
            }

        }

        public void StopService()
        {
            server.Stop();
            IsRunning = false;
            GlobalInfo.NodeInfo.Redis.KeyDelete("EasyNChat_" + GlobalInfo.NodeInfo.NodeName + "_User");
            log.LogInformation("WebSocket Chat Service Is Stoped.");
        }
    }
}
