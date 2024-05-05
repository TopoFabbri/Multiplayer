using System;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;

namespace Network
{
    public class NetworkManager : MonoBehaviourSingleton<NetworkManager>, IReceiveData
    {
        [SerializeField] private TextMeshProUGUI clientsTxt;
        [SerializeField] private TextMeshProUGUI msTxt;

        public IPAddress ipAddress { get; private set; }

        public int port { get; private set; }

        public bool isServer { get; private set; }

        public const float TimeOut = 10f;

        public Action<byte[]> OnReceiveEvent;

        private int thisId = -1;

        private UdpConnection connection;

        private readonly Dictionary<int, Client> clients = new();
        private readonly Dictionary<IPEndPoint, int> ipToId = new();
        private readonly List<int> ids = new();

        private Client client;

        int clientIds = 0; // This id should be generated during first handshake

        public void StartServer(int port)
        {
            isServer = true;
            this.port = port;
            connection = new UdpConnection(port, this);
        }

        public void StartClient(IPAddress ip, int port)
        {
            isServer = false;

            this.port = port;
            this.ipAddress = ip;

            connection = new UdpConnection(ip, port, this);

            NetHandShake handShake = new();
            handShake.Add(0);

            SendToServer(handShake.Serialize());
        }

        private void AddClient(IPEndPoint ip)
        {
            if (ipToId.ContainsKey(ip)) return;

            Debug.Log("Adding client: " + ip.Address);

            int id = clientIds;
            ipToId[ip] = clientIds;

            clients.Add(clientIds, new Client(ip, id, Time.realtimeSinceStartup));
            ids.Add(clientIds);

            clientIds++;

            if (isServer)
            {
                NetHandShake handShake = new();

                foreach (KeyValuePair<int, Client> client in clients)
                    handShake.Add(client.Key);

                Broadcast(handShake.Serialize());
                
                NetPing ping = new();
                ping.SetData(clients[ipToId[ip]].ms);
                
                connection.Send(ping.Serialize(), ip);
            }
        }

        private void RemoveClient(IPEndPoint ip)
        {
            if (!ipToId.ContainsKey(ip)) return;

            Debug.Log("Removing client: " + ip.Address);
            clients.Remove(ipToId[ip]);
        }

        public void OnReceiveData(byte[] data, IPEndPoint ip)
        {
            switch (MessageHandler.Instance.GetMessageType(data))
            {
                case MessageType.HandShake:
                    if (isServer)
                    {
                        AddClient(ip);
                    }
                    else
                    {
                        NetHandShake hs = new();
                        int[] newIds = hs.Deserialize(data).ToArray();

                        if (thisId == -1)
                        {
                            thisId = newIds[newIds.Length - 1];
                            clients.Add(thisId, new Client(ip, thisId, 0f));
                        }
                        
                        foreach (int id in newIds)
                        {
                            if (!ids.Contains(id))
                                ids.Add(id);
                        }
                    }

                    break;

                case MessageType.Console:
                    OnReceiveEvent?.Invoke(data);
                    break;

                case MessageType.Position:
                    break;

                case MessageType.PingPong:
                    if (isServer)
                    {
                        NetPing ping = new();
                        ping.SetData(clients[ipToId[ip]].ms);
                        
                        connection.Send(ping.Serialize(), ip);
                        
                        clients[ipToId[ip]].ResetMs();
                    }
                    else
                    {
                        NetPing ping = new();
                        NetPing pong = new();

                        clients[thisId].SetMs(ping.Deserialize(data));

                        SendToServer(pong.Serialize());
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SendToServer(byte[] data)
        {
            connection.Send(data);
        }

        public void Broadcast(byte[] data)
        {
            using (var iterator = clients.GetEnumerator())
            {
                while (iterator.MoveNext())
                {
                    connection.Send(data, iterator.Current.Value.ipEndPoint);
                }
            }
        }

        private void Update()
        {
            // Flush the data in main thread
            connection?.FlushReceiveData();

            clientsTxt.text = "Clients: " + ids.Count;
            
            if (thisId >= 0)
                msTxt.text = "ms: " + clients[thisId].ms;

            CheckClientsTimeOut();
        }

        private void CheckClientsTimeOut()
        {
            for (int i = 0; i < clients.Count; i++)
            {
                Client client = clients[i];
                client.ms += Time.deltaTime;
                clients[i] = client;

                // if (client.ms > TimeOut)
                //     RemoveClient(client.ipEndPoint);
            }
        }
    }
}