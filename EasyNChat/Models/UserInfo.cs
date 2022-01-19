using System;
using System.Collections.Generic;
using System.Text;

namespace EasyNChat.Models
{
    public class UserInfo<T> where T : class
    {
        public Guid SessionId { get; set; }
        public string UserId { get; set; }
        public string ConnectNodeName { get; set; }
        public T User { get; set; }
    }
}
