using System;
using System.Collections.Generic;
using Network.MessageTypes;

namespace Network
{
    public class NetHandShake : Message<List<int>>
    {
        private readonly List<int> data = new();

        public override List<int> Deserialize(byte[] message)
        {
            List<int> outData = new();

            for (int i = MessageData.GetSize(); i < message.Length; i += 4)
            {
                byte[] curInt = {message[i], message[i + 1], message[i + 2], message[i + 3]};
                outData.Add(BitConverter.ToInt32(curInt, 0));
            }

            return outData;
        }

        public override MessageType GetMessageType()
        {
            return MessageType.HandShake;
        }

        public override byte[] Serialize(bool fromServer)
        {
            List<byte> outData = new();

            messageData.type = GetMessageType();
            messageData.fromServer = fromServer;
            
            outData.AddRange(messageData.Serialize());

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