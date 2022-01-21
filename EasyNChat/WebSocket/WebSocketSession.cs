using EasyNChat.Models;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace EasyNChat.WebSocket
{
    public class WebSocketSession : WsSession
    {
        private string UserId { get; set; }
        public WebSocketSession(WsServer server) : base(server)
        {
        }

        public WebSocketSession()
        {

        }
        public void SetServer(WsServer server)
        {
            base.SetWsServer(server);
        }
        public override void OnWsConnected(HttpRequest request)
        {
            if (!RequestIsPermission(request))
            {
                SendText(NotPermissionReturn());
                Close(0);
            }
            else
            {
                var userinfo = SetUserInfo(request);
                userinfo.ConnectType = ConnectType.WebSocket;
                UserId = userinfo.UserId;
                GlobalInfo.AddUserInfo(userinfo);
                base.OnWsConnected(request);
            }
        }

        public override void OnWsClose(byte[] buffer, long offset, long size)
        {
            GlobalInfo.DeleteUserInfo(UserId);
            base.OnWsClose(buffer, offset, size);
        }

        public override void OnWsReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            var data = JsonSerializer.Deserialize<MessageData>(message);
            var ToUserInfo = GlobalInfo.GetUserInfo(data.ToUserId);
            if (ToUserInfo != null)
            {
                GlobalInfo.NodeInfo.Redis.Publish(ToUserInfo.RecieveSubName, message);
                SendMessage(new MessageData() { Message = "Send Success", DataType = MessageDataType.Text });
            }
            else
            {
                SendMessage(new MessageData() { Message = "Reciever Not Online,Message Will Send When The Reciever Online", DataType = MessageDataType.Text });
            }
        }

        public virtual bool RequestIsPermission(HttpRequest request)
        {
            return true;
        }
        public virtual string NotPermissionReturn()
        {
            return "NOT PERMISSION";
        }

        public virtual UserInfo SetUserInfo(HttpRequest request)
        {
            UserInfo userInfo = new UserInfo();
            if (request.Url.Split('?').Length >= 2)
            {
                foreach (var paras in request.Url.Split('?')[1].Split('&'))
                {
                    string para = paras.Split('=')[0];
                    string val = paras.Split('=')[1];
                    if (para == "userid")
                    {

                        userInfo.UserId = val;
                        userInfo.SessionId = Id;
                        userInfo.ConnectNodeName = GlobalInfo.NodeInfo.NodeName;
                        userInfo.RecieveSubName = GlobalInfo.NodeInfo.RecieveSubName;
                    }
                }
            }
            return userInfo;
        }

        public void SendMessage(MessageData message)
        {
            if (message != null)
            {
                if (message.DataType == MessageDataType.Text)
                {
                    SendText(JsonSerializer.Serialize(message));
                }
                else
                {

                }
            }
        }
    }
}
