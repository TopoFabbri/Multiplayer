using System;
using System.Collections.Generic;

namespace Network
{
    public class NetConsole : IMessage<string>
    {
        public string data;

        public string Deserialize(byte[] message)
        {
            string outData = "";

            for (int i = 4; i < message.Length; i++)
                outData += (char)message[i];

            return outData;
        }

        public MessageType GetMessageType()
        {
            return MessageType.Console;
        }

        public byte[] Serialize()
        {
            List<byte> outData = new();

            outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));

            foreach (char letter in data)
                outData.Add((byte)letter);

            return outData.ToArray();
        }
    }
}