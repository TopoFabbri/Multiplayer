using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    public class NetVector3 : IMessage<(int id, Vector3 pos)>
    {
        private (int id, Vector3 pos) data;

        public NetVector3((int id, Vector3 pos) data)
        {
            this.data = data;
        }

        public (int, Vector3) Deserialize(byte[] message)
        {
            (int id, Vector3 pos) outData;

            outData.id = BitConverter.ToInt32(message, 4);
            outData.pos.x = BitConverter.ToSingle(message, 8);
            outData.pos.y = BitConverter.ToSingle(message, 12);
            outData.pos.z = BitConverter.ToSingle(message, 16);

            return outData;
        }

        public MessageType GetMessageType()
        {
            return MessageType.Position;
        }

        public byte[] Serialize()
        {
            List<byte> outData = new();

            outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
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