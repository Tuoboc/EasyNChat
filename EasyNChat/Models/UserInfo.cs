using System;
using System.Collections.Generic;
using System.Text;

namespace EasyNChat.Models
{
    public class UserInfo
    {
        public Guid SessionId { get; set; }
        public string UserId { get; set; }
        public string ConnectNodeName { get; set; }
        public object User { get; set; }
        public string RecieveSubName { get; set; }
        public ConnectType ConnectType { get; set; }
    }

    public enum ConnectType
    {
        WebSocket,
        Socket
    }
}
