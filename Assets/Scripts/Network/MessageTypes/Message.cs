using System;
using System.Collections.Generic;

namespace Network.MessageTypes
{
    public abstract class Message<T>
    {
        protected MessageData messageData;
        
        public abstract MessageType GetMessageType();
        public abstract byte[] Serialize(bool fromServer);
        public abstract T Deserialize(byte[] message);
    }

    public struct MessageData
    {
        public MessageType type;
        public bool fromServer;

        public byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(BitConverter.GetBytes((int)type));
            outData.AddRange(BitConverter.GetBytes(fromServer));
            
            return outData.ToArray();
        }

        public static int GetSize()
        {
            int size = 0;
            
            size += sizeof(MessageType);
            size += sizeof(bool);
            
            return size;
        }
    }
}