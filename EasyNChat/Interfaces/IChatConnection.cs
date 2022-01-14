using System;
using System.Collections.Generic;
using System.Text;

namespace EasyNChat.Interfaces
{
    interface IChatConnection
    {
        public void StartService();
        public void StopService();
    }
}
