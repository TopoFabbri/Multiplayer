using System;
using System.Collections.Generic;

namespace Network.MessageTypes
{
    public class NetSpawnRequest : Message<int>
    {
        private int id;
        
        public override MessageType GetMessageType()
        {
            return MessageType.SpawnRequest;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            
            messageData.type = GetMessageType();
            
            outData.AddRange(messageData.Serialize());
            outData.AddRange(BitConverter.GetBytes(id));
            
            return outData.ToArray();
        }

        public override int Deserialize(byte[] message)
        {
            id = BitConverter.ToInt32(message, MessageData.GetSize());
            
            return id;
        }
        
        public void SetId(int id)
        {
            this.id = id;
        }
    }
}