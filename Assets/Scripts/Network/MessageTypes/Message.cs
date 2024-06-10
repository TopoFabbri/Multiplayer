using System;
using System.Collections.Generic;

namespace Network.MessageTypes
{
    public abstract class Message<T>
    {
        protected MessageData messageData;
        
        public abstract MessageType GetMessageType();
        public abstract byte[] Serialize();
        public abstract T Deserialize(byte[] message);
    }

    public struct MessageData
    {
        public MessageType type;

        public byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(BitConverter.GetBytes((int)type));
            
            return outData.ToArray();
        }

        public static int GetSize()
        {
            int size = 0;
            
            size += sizeof(MessageType);
            
            return size;
        }
    }
}