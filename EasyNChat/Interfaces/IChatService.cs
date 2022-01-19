using EasyNChat.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyNChat.Interfaces
{
    interface IChatService
    {
        public void StartService(ServerNodeInfo nodeinfo);
        public void StopService();
        public bool IsRunning { get; set; }
    }
}
