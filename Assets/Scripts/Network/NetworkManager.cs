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

        public Action<byte[]> OnReceiveEvent;

        private UdpConnection connection;

        private Server server;
        private Client client;

        int clientIds = 0;

        public void StartServer(int port)
        {
            isServer = true;
            
            this.port = port;
            
            connection = new UdpConnection(port, this);
            
            server = new Server();
        }

        public void StartClient(IPAddress ip, int port)
        {
            isServer = false;

            this.port = port;
            ipAddress = ip;

            connection = new UdpConnection(ip, port, this);
            
            client = new Client(new IPEndPoint(ip, port), clientIds, Time.time);
        }

        public void OnReceiveData(byte[] data, IPEndPoint ip)
        {
        }

        public void SendToServer(byte[] data)
        {
            connection.Send(data);
        }

        public void Broadcast(byte[] data)
        {
            if (!isServer)
                return;

            List<Client> clients = server.GetClientsList();
            
            foreach (Client clientInList in clients)
                connection.Send(data, clientInList.ipEndPoint);
        }
        
        public void AddClient(IPEndPoint ip)
        {
            if (!isServer)
                return;
            
            server.AddClient(ip);
        }

        private void Update()
        {
            connection?.FlushReceiveData();
            
            UpdateUI();
            CheckClientsTimeOut();
        }

        private void UpdateUI()
        {
            if (server != null)
                clientsTxt.text = "Clients:" + server.GetClientsIdList().Count;
            
            if (client != null)
                msTxt.text = "ms: " + client.Ms;
        }
        
        private void CheckClientsTimeOut()
        {
            if (!isServer)
                return;
            
            server.CheckClientsTimeOut();
        }
    }
}