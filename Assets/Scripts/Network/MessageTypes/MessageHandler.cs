using System;
using System.Net;

namespace Network
{
    public class MessageHandler
    {
        private static MessageHandler _instance;

        public static MessageHandler Instance =>  _instance ??= new MessageHandler();
        
        public static MessageType GetMessageType(byte[] data)
        {
            return (MessageType)BitConverter.ToInt32(data, 0);
        }
    }
}