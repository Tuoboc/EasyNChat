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
                userinfo.SetConnectionInfo(Id, GlobalInfo.NodeInfo.NodeName, GlobalInfo.NodeInfo.RecieveSubName, ConnectType.WebSocket);
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

        /// <summary>
        /// Check if the connection is allowed to continue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual bool RequestIsPermission(HttpRequest request)
        {
            return true;
        }
        /// <summary>
        /// if RequestIsPermission method return false,this string will return to the client
        /// </summary>
        /// <returns></returns>
        public virtual string NotPermissionReturn()
        {
            return "NOT PERMISSION";
        }
        /// <summary>
        /// save the userinfo to the redis
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual UserInfo SetUserInfo(HttpRequest request)
        {
            return null;
        }

        public virtual void BeforeSendMessage(MessageData message)
        {

        }
        public virtual void AfterSendMessage(MessageData message)
        {

        }
        public void SendMessage(MessageData message)
        {
            BeforeSendMessage(message);
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
            AfterSendMessage(message);
        }
    }
}
