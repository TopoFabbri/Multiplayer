using System;
using System.Net;
using Network.MessageTypes;
using TMPro;
using UnityEngine;
using Utils;

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

        public Server server;
        private Client client;

        public void StartServer(IPAddress ip, int port)
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
            Debug.Log("Received " + MessageHandler.TypeToString(MessageHandler.GetMessageData(data).type) + " from: " +
                      (server != null ? server.GetIdByIp(ip) : "Server"));

            client?.HandleMessage(data);
            server?.HandleMessage(data, ip);
        }

        public void Broadcast(byte[] data)
        {
            if (!isServer)
                return;

            server?.Broadcast(data);
        }

        public void SendToServer(byte[] data)
        {
            connection?.Send(data);
        }

        public void SendToClient(byte[] data, IPEndPoint ip)
        {
            connection?.Send(data, ip);
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

            if (client == null) return;

            showMsTimer += Time.deltaTime;

            if (!isServer)
                clientsTxt.text = "ID: " + client.id;

            if (showMsTimer >= showMsFreq)
            {
                while (showMsTimer > showMsFreq)
                    showMsTimer -= showMsFreq;

                msTxt.text = "ms: " + client.Ms;
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