using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Network.MessageTypes;
using Players;
using Spawn;
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
        
        public void SendToServer(byte[] message)
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
                    HandlePosition(message);
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

        private void HandlePosition(byte[] pos)
        {
            NetVector3 vec3 = new();
            
            (int id, Vector3 pos) idPos = vec3.Deserialize(pos);
            
            EnemyManager.Instance.UpdatePosition(idPos.id, idPos.pos);
        }

        private void HandleConsole(byte[] message)
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

        private void HandlePing(byte[] message)
        {
            NetPing pong = new();

            Ms = pong.Deserialize(message);
            
            SendToServer(pong.Serialize());
        }
    }
}