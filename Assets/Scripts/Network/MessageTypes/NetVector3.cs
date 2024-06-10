using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network.MessageTypes
{
    public class NetVector3 : Message<(int id, Vector3 pos)>
    {
        private (int id, Vector3 pos) data;

        public NetVector3()
        {
        }
        
        public NetVector3((int id, Vector3 pos) data)
        {
            this.data = data;
        }

        public override (int, Vector3) Deserialize(byte[] message)
        {
            (int id, Vector3 pos) outData;

            outData.id = BitConverter.ToInt32(message, MessageData.GetSize());
            outData.pos.x = BitConverter.ToSingle(message, MessageData.GetSize() + sizeof(float));
            outData.pos.y = BitConverter.ToSingle(message, MessageData.GetSize() + sizeof(float) * 2);
            outData.pos.z = BitConverter.ToSingle(message, MessageData.GetSize() + sizeof(float) * 3);

            return outData;
        }

        public override MessageType GetMessageType()
        {
            return MessageType.Position;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();

            messageData.type = GetMessageType();
            
            outData.AddRange(messageData.Serialize());
            outData.AddRange(BitConverter.GetBytes(data.id));
            outData.AddRange(BitConverter.GetBytes(data.pos.x));
            outData.AddRange(BitConverter.GetBytes(data.pos.y));
            outData.AddRange(BitConverter.GetBytes(data.pos.z));

            return outData.ToArray();
        }
        
        public void SetId(int id)
        {
            data.id = id;
        }
        
        public void SetPos(Vector3 pos)
        {
            data.pos = pos;
        }
    }
}