using System;
using System.Collections.Generic;

namespace Network
{
    public class NetPing : IMessage<byte>
    {
        private byte data;

        public MessageType GetMessageType()
        {
            return MessageType.PingPong;
        }

        public byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
            outData.Add(data);
            
            return outData.ToArray();
        }

        public byte Deserialize(byte[] message)
        {
            data = message[4];

            return data;
        }
    }
}