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
        [SerializeField] private float showMsFreq = 1f;

        private float showMsTimer = 1f;

        public IPAddress ipAddress { get; private set; }
        public int port { get; private set; }
        public bool isServer { get; private set; }

        public Action<byte[]> OnReceiveEvent;

        private UdpConnection connection;

        private Server server;
        private Client client;

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

            client = new Client(new IPEndPoint(ip, port), -1, Time.time);

            NetHandShake hs = new();
            SendToServer(hs.Serialize());
        }

        public void OnReceiveData(byte[] data, IPEndPoint ip)
        {
            client?.HandleMessage(data);
            server?.HandleMessage(data, ip);
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

        public void SendToClient(byte[] data, IPEndPoint ip)
        {
            connection.Send(data, ip);
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
            {
                clientsTxt.text = "Clients:" + server.GetClientsIdList().Count;
            }
            else if (client != null)
            {
                showMsTimer += Time.deltaTime;
                clientsTxt.text = "ID: " + client.id;

                if (showMsTimer >= showMsFreq)
                {
                    while (showMsTimer > showMsFreq)
                        showMsTimer -= showMsFreq;

                    msTxt.text = "ms: " + client.Ms;
                }
            }
        }

        private void CheckClientsTimeOut()
        {
            if (!isServer)
                return;

            server.CheckClientsTimeOut();
        }
    }
}