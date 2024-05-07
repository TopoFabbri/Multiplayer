using System;
using System.Collections.Generic;

namespace Network
{
    public class NetSpawnRequest : IMessage<int>
    {
        private int id;
        
        public MessageType GetMessageType()
        {
            return MessageType.SpawnRequest;
        }

        public byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
            outData.AddRange(BitConverter.GetBytes(id));
            
            return outData.ToArray();
        }

        public int Deserialize(byte[] message)
        {
            id = BitConverter.ToInt32(message, 4);
            
            return id;
        }
    }
}