using System;
using System.Collections.Generic;
using System.Text;

namespace EasyNChat.Models
{
    public class MessageData
    {
        public MessageDataType DataType { get; set; }
        public string UserId { get; set; }
        public string ToUserId { get; set; }
        public byte[] Data { get; set; }
        public string Message { get; set; }
    }

    public enum MessageDataType
    {
        Text = 10000,
        Image = 10001,
        Sound = 10002
    }
}
