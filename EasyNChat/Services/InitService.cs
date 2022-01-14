using EasyNChat.Models;
using Microsoft.Extensions.Configuration;
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
    internal class InitService : IHostedService
    {
        IConfiguration config;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly LogService log;
        private readonly SendMessageService sendService;
        private readonly WebSocketService socketService;

        public InitService(IConfiguration configuration, IHostApplicationLifetime appLifetime, LogService logger, SendMessageService sendMessageService, WebSocketService webSocketService)
        {
            config = configuration;
            _appLifetime = appLifetime;
            log = logger;
            sendService = sendMessageService;
            socketService = webSocketService;
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

            var nodeconfigList = nodeInfo.Redis.StringGet("EasyNChat_Servers_Info");
            List<ServerNodeInfo> nodeList = new List<ServerNodeInfo>();
            if (!nodeconfigList.IsNullOrEmpty)
            {
                nodeList = JsonSerializer.Deserialize<List<ServerNodeInfo>>(nodeconfigList);
            }
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
            nodeInfo.Redis.StringSet("EasyNChat_Servers_Info", JsonSerializer.Serialize(nodeList));
            nodeInfo.IsRunning = true;

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            InitNodeInfo();
            if (GlobalInfo.NodeInfo.IsRunning)
            {
                GlobalInfo.NodeInfo.Sub.Subscribe(GlobalInfo.NodeInfo.RecieveSubName).OnMessage(msg => sendService.SendMessage(msg));
                log.LogInformation("SendMessageService is Started.");
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            if (GlobalInfo.NodeInfo.IsRunning)
            {
                var nodeconfigList = GlobalInfo.NodeInfo.Redis.StringGet("EasyNChat_Servers_Info");
                var nodeList = JsonSerializer.Deserialize<List<ServerNodeInfo>>(nodeconfigList);
                var node = nodeList.FirstOrDefault(a => a.RandomId == GlobalInfo.NodeInfo.RandomId);
                nodeList.Remove(node);
                GlobalInfo.NodeInfo.Redis.StringSet("EasyNChat_Servers_Info", JsonSerializer.Serialize(nodeList));
                GlobalInfo.NodeInfo.Sub.Unsubscribe(GlobalInfo.NodeInfo.RecieveSubName);
                log.LogInformation("SendMessageService is stoped.");
                if (socketService.IsRunning)
                {
                    socketService.StopService();
                }
            }
            return Task.CompletedTask;
        }
    }
}
