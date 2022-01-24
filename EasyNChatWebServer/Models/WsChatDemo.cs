using EasyNChat.Models;
using EasyNChat.WebSocket;

namespace EasyNChatWebServer.Models
{
    public class WsChatDemo : WebSocketSession
    {
        public override bool RequestIsPermission(NetCoreServer.HttpRequest request)
        {
            if (!request.Url.Contains("userid"))
            {
                return false;
            }
            else
                return true;
        }

        public override string NotPermissionReturn()
        {
            return "if RequestIsPermission method return false,this string will return to the client";
        }

        public override UserInfo SetUserInfo(NetCoreServer.HttpRequest request)
        {
            UserInfo user = null;
            if (request.Url.Split('?').Length >= 2)
            {
                foreach (var paras in request.Url.Split('?')[1].Split('&'))
                {
                    string para = paras.Split('=')[0];
                    string val = paras.Split('=')[1];
                    if (para == "userid")
                    {
                        var customuser = new { sex = "man", level = 1 };
                        user = new UserInfo(val, customuser);
                    }
                }
            }
            return user;
        }
    }
}
