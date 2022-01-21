using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace EasyNChat.Models
{
    public static class GlobalInfo
    {
        public static ServerNodeInfo NodeInfo { get; set; }

        #region UserManage
        public static void AddUserInfo(UserInfo user)
        {
            NodeInfo.Redis.HashSet("EasyNChat_" + NodeInfo.NodeName + "_User", user.UserId, JsonSerializer.Serialize(user));
        }
        public static void DeleteUserInfo(string userid)
        {
            NodeInfo.Redis.HashDelete("EasyNChat_" + NodeInfo.NodeName + "_User", userid);
        }
        public static void DeleteUserInfo()
        {
            NodeInfo.Redis.KeyDelete("EasyNChat_" + NodeInfo.NodeName + "_User");
        }
        public static UserInfo GetUserInfo(string userid)
        {
            var nodelist = GetServerInfo();
            foreach (var item in nodelist)
            {
                var user = NodeInfo.Redis.HashGet("EasyNChat_" + item.NodeName + "_User", userid);
                if (user.HasValue)
                    return JsonSerializer.Deserialize<UserInfo>(user);
            }
            return null;
        }
        #endregion

        #region ServerManage
        public static void ChangeServerInfo(List<ServerNodeInfo> nodeList)
        {
            if (nodeList.Count() > 0)
                NodeInfo.Redis.StringSet("EasyNChat_Servers_Info", JsonSerializer.Serialize(nodeList));
            else
                NodeInfo.Redis.KeyDelete("EasyNChat_Servers_Info");
        }
        public static void DeleteServerInfo()
        {
            var nodeList = GetServerInfo();
            var node = nodeList.FirstOrDefault(a => a.RandomId == NodeInfo.RandomId);
            if (node != null)
                nodeList.Remove(node);
            ChangeServerInfo(nodeList);

        }
        public static List<ServerNodeInfo> GetServerInfo()
        {
            var nodeconfigList = NodeInfo.Redis.StringGet("EasyNChat_Servers_Info");
            List<ServerNodeInfo> nodeList = new List<ServerNodeInfo>();
            if (!nodeconfigList.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<List<ServerNodeInfo>>(nodeconfigList);
            }
            else
            {
                return new List<ServerNodeInfo>();
            }
        }
        #endregion
    }
}
