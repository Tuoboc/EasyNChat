using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace EasyNChat.Models
{
    /// <summary>
    /// chatserver running information
    /// </summary>
    public class ServerNodeInfo
    {
        public string NodeName { get; set; }
        public DateTime StartTime { get; } = DateTime.Now;

        public int RandomId { get; set; } = new Random().Next(1, 9999);
        [JsonIgnore]
        public IDatabase Redis { get; set; }
        [JsonIgnore]
        public ISubscriber Sub { get; set; }
        public int WsPort { get; set; }
        public string RecieveSubName { get { return "EasyNChat_" + NodeName + "_RecieveMessage"; } }

        public bool IsRunning { get; set; }
    }
}
