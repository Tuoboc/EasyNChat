using EasyNChat.Models;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace EasyNChat.WebSocket
{
    internal class WebSocketManager : WsServer
    {
        public WebSocketManager(IPAddress address, int port) : base(address, port)
        {
        }

        protected override void OnConnected(TcpSession session)
        {
            base.OnConnected(session);
        }

        protected override TcpSession CreateSession()
        {
            var session = new WebSocketSession(this);

            return session;
        }
    }
}
