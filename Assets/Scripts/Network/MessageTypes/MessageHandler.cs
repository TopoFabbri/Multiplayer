using System;

namespace Network.MessageTypes
{
    public class MessageHandler
    {
        private static MessageHandler _instance;

        public static MessageHandler Instance =>  _instance ??= new MessageHandler();
        
        public static MessageData GetMessageData(byte[] data)
        {
            MessageData outData = new();

            int size = 0;
            
            outData.type = (MessageType)BitConverter.ToInt32(data, size);
            size += sizeof(int);
            
            return outData;
        }
        
        public static string TypeToString(MessageType type)
        {
            return Enum.GetName(typeof(MessageType), type);
        }
    }
}