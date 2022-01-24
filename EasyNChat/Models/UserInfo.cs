using System;
using System.Collections.Generic;
using System.Text;

namespace EasyNChat.Models
{
    public class UserInfo
    {
        public Guid SessionId { get; private set; }
        public string UserId { get; set; }
        public string ConnectNodeName { get; private set; }
        public object User { get; set; }
        public string RecieveSubName { get; private set; }
        public ConnectType ConnectType { get; private set; }

        public void SetConnectionInfo(Guid sessionid, string connectnodename, string recievesubname, ConnectType connecttype)
        {
            SessionId = sessionid;
            ConnectNodeName = connectnodename;
            RecieveSubName = recievesubname;
            ConnectType = connecttype;
        }
        public UserInfo(string userid, object user)
        {
            UserId = userid;
            User = user;
        }
    }

    public enum ConnectType
    {
        WebSocket,
        Socket
    }
}
