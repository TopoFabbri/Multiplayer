using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Network
{
    public class Server
    {
        private readonly Dictionary<int, Client> clients = new();
        private readonly Dictionary<IPEndPoint, int> ipToId = new();

        private int clientCount;
        
        public void AddClient(IPEndPoint ip)
        {
            Client client = new(ip, clientCount, Time.time);

            ipToId.TryAdd(ip, clientCount);
            clients.TryAdd(clientCount, client);
            
            clientCount++;
        }
        
        public void RemoveClient(IPEndPoint ip)
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
            for (int i = 0; i < clients.Count; i++)
            {
                Client client = clients[i];
                client.Ms += Time.deltaTime;
                clients[i] = client;
            }
        }
    }
}