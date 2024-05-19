using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Network.MessageTypes;
using UnityEngine;

namespace Network
{
    public class Client
    {
        public readonly IPEndPoint ipEndPoint;
        
        public float timeStamp;
        public int id;
        
        private List<int> clientIds = new();
        
        public static event Action Connected; 
        
        public float Ms { get; set; }

        public Client(IPEndPoint ipEndPoint, int id, float timeStamp)
        {
            Ms = 0f;
            this.timeStamp = timeStamp;
            this.id = id;
            this.ipEndPoint = ipEndPoint;
        }
        
        protected virtual void SendToServer(byte[] message)
        {
            NetworkManager.Instance.SendToServer(message);
        }

        public void HandleMessage(byte[] message)
        {
            switch (MessageHandler.GetMessageData(message).type)
            {
                case MessageType.HandShake:
                    HandleHandShake(message);
                    break;
                
                case MessageType.Console:
                    HandleConsole(message);
                    break;
                
                case MessageType.Position:
                    break;
                
                case MessageType.PingPong:
                    HandlePing(message);
                    break;

                case MessageType.SpawnRequest:
                    HandleSpawnRequest(message);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        protected virtual void HandleConsole(byte[] message)
        {
            NetworkManager.Instance.OnReceiveEvent.Invoke(message);
        }

        private void HandleHandShake(byte[] message)
        {
            NetHandShake hs = new();
            
            clientIds = hs.Deserialize(message);
            
            if (id == -1)
                id = clientIds.Last();
            
            Connected?.Invoke();
        }

        private void HandleSpawnRequest(byte[] message)
        {
            NetSpawnRequest sr = new();
            
            Spawner.Instance.Spawn(sr.Deserialize(message));
        }
        
        protected virtual void HandlePing(byte[] message)
        {
            NetPing pong = new();

            Ms = pong.Deserialize(message);
            
            SendToServer(pong.Serialize(false));
        }
    }
    
    public class SvClient : Client
    {
        public SvClient(IPEndPoint ipEndPoint, int id, float timeStamp) : base(ipEndPoint, id, timeStamp)
        {
        }
        
        protected override void SendToServer(byte[] message)
        {
            NetworkManager.Instance.server.HandleMessage(message, ipEndPoint);
        }

        protected override void HandlePing(byte[] message)
        {
        }
        
        protected override void HandleConsole(byte[] message)
        {
        }
    }
}