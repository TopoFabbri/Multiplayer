using System;
using System.Collections.Generic;

namespace Network.MessageTypes
{
    public class NetConsole : Message<string>
    {
        public string data;

        public override string Deserialize(byte[] message)
        {
            string outData = "";

            for (int i = MessageData.GetSize(); i < message.Length; i++)
                outData += (char)message[i];

            return outData;
        }

        public override MessageType GetMessageType()
        {
            return MessageType.Console;
        }

        public override byte[] Serialize(bool fromServer)
        {
            List<byte> outData = new();

            messageData.type = GetMessageType();
            messageData.fromServer = fromServer;
            
            outData.AddRange(messageData.Serialize());

            foreach (char letter in data)
                outData.Add((byte)letter);

            return outData.ToArray();
        }
    }
}