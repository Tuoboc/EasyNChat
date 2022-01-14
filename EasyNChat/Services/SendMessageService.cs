using EasyNChat.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EasyNChat.Services
{
    public class SendMessageService
    {
        IConfiguration config;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly LogService log;

        public SendMessageService(IConfiguration configuration, IHostApplicationLifetime appLifetime, LogService logger)
        {
            config = configuration;
            _appLifetime = appLifetime;
            log = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (GlobalInfo.NodeInfo.IsRunning)
            {
                GlobalInfo.NodeInfo.Sub.Subscribe(GlobalInfo.NodeInfo.RecieveSubName).OnMessage(msg => SendMessage(msg));
                log.LogInformation("SendMessageService is stoped.");
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (GlobalInfo.NodeInfo.IsRunning)
            {
                GlobalInfo.NodeInfo.Sub.Unsubscribe(GlobalInfo.NodeInfo.RecieveSubName);
                log.LogInformation("SendMessageService is stoped.");
            }
            return Task.CompletedTask;
        }

        public void SendMessage(ChannelMessage message)
        {
            log.LogInformation(message.Message.ToString());
        }
    }
}
