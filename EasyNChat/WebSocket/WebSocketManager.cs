using EasyNChat.Models;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace EasyNChat.WebSocket
{
    public class WebSocketManager<T> : WsServer where T : WebSocketSession, new()
    {

        public WebSocketManager() : base(IPAddress.Any, GlobalInfo.NodeInfo.WsPort)
        {

        }

        protected override void OnConnected(TcpSession session)
        {
            base.OnConnected(session);
        }

        protected override TcpSession CreateSession()
        {
            var session = new T();
            session.SetServer(this);
            return session;
        }
    }
}
