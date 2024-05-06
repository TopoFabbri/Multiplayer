using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Network
{
    public class Client
    {
        public readonly IPEndPoint ipEndPoint;
        
        public float timeStamp;
        public int id;
        
        private List<int> clientIds = new();
        
        public float Ms { get; set; }

        public Client(IPEndPoint ipEndPoint, int id, float timeStamp)
        {
            Ms = 0f;
            this.timeStamp = timeStamp;
            this.id = id;
            this.ipEndPoint = ipEndPoint;
        }
        
        private static void SendToServer(byte[] message)
        {
            NetworkManager.Instance.SendToServer(message);
        }

        public void HandleMessage(byte[] message)
        {
            switch (MessageHandler.GetMessageType(message))
            {
                case MessageType.HandShake:
                    HandleHandShake(message);
                    break;
                
                case MessageType.Console:
                    NetworkManager.Instance.OnReceiveEvent.Invoke(message);
                    break;
                
                case MessageType.Position:
                    break;
                
                case MessageType.PingPong:
                    HandlePing(message);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleHandShake(byte[] message)
        {
            NetHandShake hs = new();
            
            clientIds = hs.Deserialize(message);
            
            if (id == -1)
                id = clientIds.Last();
        }
        
        private void HandlePing(byte[] message)
        {
            NetPing pong = new();

            Ms = pong.Deserialize(message);
            
            SendToServer(pong.Serialize());
        }
    }
}