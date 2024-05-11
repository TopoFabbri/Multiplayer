using System;
using System.Collections.Generic;

namespace Network
{
    public class NetHandShake : IMessage<List<int>>
    {
        private readonly List<int> data = new();

        public List<int> Deserialize(byte[] message)
        {
            List<int> outData = new();

            for (int i = 4; i < message.Length; i += 4)
            {
                byte[] curInt = {message[i], message[i + 1], message[i + 2], message[i + 3]};
                outData.Add(BitConverter.ToInt32(curInt, 0));
            }

            return outData;
        }

        public MessageType GetMessageType()
        {
            return MessageType.HandShake;
        }

        public byte[] Serialize()
        {
            List<byte> outData = new();

            outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));

            foreach (int i in data)
                outData.AddRange(BitConverter.GetBytes(i));

            return outData.ToArray();
        }
    
        public void Add(int id)
        {
            data.Add(id);
        }
    }
}