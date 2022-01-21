using EasyNChat.Interfaces;
using EasyNChat.Models;
using EasyNChat.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EasyNChat.Services
{
    public class EasyNChatService<T> where T : WebSocketSession, new()
    {
        IConfiguration config;
        private readonly LogService log;
        IServiceProvider service;
        IHostApplicationLifetime lifetime;

        public EasyNChatService(IConfiguration configuration, LogService logger, IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime)
        {
            config = configuration;
            log = logger;
            service = serviceProvider;
            lifetime = applicationLifetime;
        }
        public EasyNChatService(WsEasyNChatConfig config)
        {

        }


        private void InitNodeInfo()
        {
            ServerNodeInfo nodeInfo = new ServerNodeInfo();
            GlobalInfo.NodeInfo = nodeInfo;
            var WsPort = config.GetSection("ChatWsPort").Value;
            nodeInfo.WsPort = string.IsNullOrWhiteSpace(WsPort) ? 5144 : Convert.ToInt32(WsPort);
            var redisHost = config.GetSection("RedisHost").Value;
            if (string.IsNullOrWhiteSpace(redisHost))
            {
                log.LogError("Can't find 'RedisHost' config section in the config files.");
                return;
            }
            nodeInfo.NodeName = Environment.GetEnvironmentVariable("NodeName");
            nodeInfo.NodeName = string.IsNullOrWhiteSpace(nodeInfo.NodeName) ? config.GetSection("NodeName").Value : nodeInfo.NodeName;
            try
            {
                ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect(redisHost);
                nodeInfo.Redis = redisConnection.GetDatabase();
                nodeInfo.Sub = redisConnection.GetSubscriber();
            }
            catch
            {
                log.LogError("connect redis server failure.");
                return;
            }

            var nodeList = GlobalInfo.GetServerInfo();
            if (!string.IsNullOrWhiteSpace(nodeInfo.NodeName))
            {
                if (nodeList.Count(a => a.NodeName == nodeInfo.NodeName) > 0)
                {
                    log.LogError(nodeInfo.NodeName + " node is already running.");
                    return;
                }
            }
            else
            {
                nodeInfo.NodeName = "DeafultNode" + (nodeList.Count() + 1);
            }

            nodeList.Add(nodeInfo);
            nodeInfo.IsRunning = true;
            GlobalInfo.ChangeServerInfo(nodeList);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            InitNodeInfo();
            if (GlobalInfo.NodeInfo.IsRunning)
            {
                GlobalInfo.NodeInfo.Sub.Subscribe(GlobalInfo.NodeInfo.RecieveSubName).OnMessage(msg => { SendMessage<T>(msg); });
                log.LogInformation("EasyNChat is running.");
            }
            lifetime.ApplicationStopping.Register(StopAsync);
            return Task.CompletedTask;
        }

        public void StopAsync()
        {

            if (GlobalInfo.NodeInfo.IsRunning)
            {
                GlobalInfo.DeleteServerInfo();
                GlobalInfo.DeleteUserInfo();
                GlobalInfo.NodeInfo.Sub.Unsubscribe(GlobalInfo.NodeInfo.RecieveSubName);
                log.LogInformation("EasyNChat is stoped.");
            }
        }


        public void SendMessage<T>(ChannelMessage message) where T : WebSocketSession, new()
        {
            var ToUser = JsonSerializer.Deserialize<MessageData>(message.Message.ToString());
            var touser = GlobalInfo.GetUserInfo(ToUser.ToUserId);
            if (touser != null && touser.ConnectNodeName == GlobalInfo.NodeInfo.NodeName)
            {
                var ws = service.GetService<WebSocketService<T>>();
                if (ws != null)
                {
                    var sendbyte = System.Text.UTF8Encoding.UTF8.GetBytes(message.Message.ToString());
                    var session = ws.server.FindSession(touser.SessionId);
                    var se = session as T;
                    se.SendMessage(ToUser);
                }
            }
        }
    }
}
