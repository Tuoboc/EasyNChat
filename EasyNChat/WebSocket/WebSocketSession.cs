using EasyNChat.Models;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyNChat.WebSocket
{
    public class WebSocketSession : WsSession
    {
        public WebSocketSession(WsServer server) : base(server)
        {
        }

        public override void OnWsConnected(HttpRequest request)
        {
            if (!RequestIsPermission())
            {
                SendText(NotPermissionReturn());
                Close(0);
            }
            else
            {
                GlobalInfo.NodeInfo.Redis.SetAdd("EasyNChat_" + GlobalInfo.NodeInfo.NodeName + "_User", Id.ToString());
                base.OnWsConnected(request);
            }
        }

        public override void OnWsClose(byte[] buffer, long offset, long size)
        {
            GlobalInfo.NodeInfo.Redis.SetRemove("EasyNChat_" + GlobalInfo.NodeInfo.NodeName + "_User", Id.ToString());
            base.OnWsClose(buffer, offset, size);
        }

        public override void OnWsReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);

        }

        public virtual bool RequestIsPermission()
        {
            return true;
        }
        public virtual string NotPermissionReturn()
        {
            return "NOT PERMISSION";
        }
    }
}
