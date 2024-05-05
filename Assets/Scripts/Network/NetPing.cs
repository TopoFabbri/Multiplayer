using System;
using System.Collections.Generic;

namespace Network
{
    public class NetPing : IMessage<float>
    {
        private float data;

        public MessageType GetMessageType()
        {
            return MessageType.PingPong;
        }

        public byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
            outData.AddRange(BitConverter.GetBytes(data));
            
            return outData.ToArray();
        }

        public float Deserialize(byte[] message)
        {
            data = message[4];

            return data;
        }
        
        public void SetData(float data)
        {
            this.data = data;
        }
    }
}