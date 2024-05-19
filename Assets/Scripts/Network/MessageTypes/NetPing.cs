using System;
using System.Collections.Generic;

namespace Network.MessageTypes
{
    public class NetPing : Message<float>
    {
        private float data;

        public override MessageType GetMessageType()
        {
            return MessageType.PingPong;
        }

        public override byte[] Serialize(bool fromServer)
        {
            List<byte> outData = new();
            
            messageData.type = GetMessageType();
            messageData.fromServer = fromServer;
            
            outData.AddRange(messageData.Serialize());
            outData.AddRange(BitConverter.GetBytes(data));
            
            return outData.ToArray();
        }

        public override float Deserialize(byte[] message)
        {
            data = message[MessageData.GetSize()];

            return data;
        }
        
        public void SetData(float data)
        {
            this.data = data;
        }
    }
}