using EasyNChat.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyNChat.Services
{
    public class LogService
    {
        private readonly ILogger<LogService> log;
        public LogService(ILogger<LogService> logger)
        {
            log = logger;
        }

        internal void LogInformation(string message)
        {
            log.LogInformation(DateTime.Now + "(" + GlobalInfo.NodeInfo.NodeName + ")" + ":" + message);
        }
        internal void LogError(string message)
        {
            log.LogError(DateTime.Now + " (" + GlobalInfo.NodeInfo.NodeName + ")" + ":" + message);
        }
    }
}
