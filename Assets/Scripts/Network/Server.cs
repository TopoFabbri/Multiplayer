using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Network
{
    public class Server
    {
        private const float TimeOut = 10f;

        private readonly Dictionary<int, Client> clients = new();
        private readonly Dictionary<IPEndPoint, int> ipToId = new();
        private readonly List<int> objIds = new();

        private int clientCount;
        
        private void AddClient(IPEndPoint ip)
        {
            Client client = new(ip, clientCount, Time.time);

            ipToId.TryAdd(ip, clientCount);
            clients.TryAdd(clientCount, client);

            clientCount++;

            NetPing ping = new();
            ping.SetData(0f);

            SendToClient(client, ping.Serialize());
        }

        private void RemoveClient(IPEndPoint ip)
        {
            clients.Remove(ipToId[ip]);
            ipToId.Remove(ip);
        }

        public List<Client> GetClientsList()
        {
            List<Client> list = new();

            foreach ((int id, Client client) in clients)
            {
                if (!list.Contains(client))
                    list.Add(client);
            }

            return list;
        }

        public List<int> GetClientsIdList()
        {
            List<int> list = new();

            foreach ((int id, Client client) in clients)
            {
                if (!list.Contains(id))
                    list.Add(id);
            }

            return list;
        }

        public void CheckClientsTimeOut()
        {
            if (clients.Count == 0)
                return;
            
            foreach ((int id, Client client) in clients)
            {
                client.Ms += Time.deltaTime;

                if (client.Ms < TimeOut) continue;
                
                RemoveClient(client.ipEndPoint);
                break;
            }
        }

        public void HandleMessage(byte[] data, IPEndPoint ip)
        {
            switch (MessageHandler.GetMessageType(data))
            {
                case MessageType.HandShake:
                    HandleHandshake(ip);
                    break;

                case MessageType.Console:
                    NetworkManager.Instance.OnReceiveEvent.Invoke(data);

                    break;

                case MessageType.Position:
                    break;

                case MessageType.PingPong:
                    HandlePing(ip);
                    break;

                case MessageType.SpawnRequest:
                    HandleSpawnRequest(ip);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void SendToClient(Client client, byte[] data)
        {
            NetworkManager.Instance.SendToClient(data, client.ipEndPoint);
        }

        private void HandleHandshake(IPEndPoint ip)
        {
            AddClient(ip);

            NetHandShake hs = new();

            foreach ((int id, Client client) in clients)
                hs.Add(id);

            NetworkManager.Instance.Broadcast(hs.Serialize());
        }

        private void HandlePing(IPEndPoint ip)
        {
            if (!clients.ContainsKey(ipToId[ip]))
                return;
            
            NetPing ping = new();
            ping.SetData(clients[ipToId[ip]].Ms);

            SendToClient(clients[ipToId[ip]], ping.Serialize());

            clients[ipToId[ip]].Ms = 0f;
        }

        private void HandleSpawnRequest(IPEndPoint ip)
        {
            int i = 0;

            while (objIds.Contains(i))
                i++;
            
            objIds.Add(i);
            
            NetSpawnRequest spawnRequest = new();
            spawnRequest.SetId(i);
        }
    }
}