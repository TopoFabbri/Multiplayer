using System;
using System.Collections.Generic;
using System.Net;
using Network;
using TMPro;
using UnityEngine;

public struct Client
{
    public float timeStamp;
    public int id;
    public IPEndPoint ipEndPoint;

    public Client(IPEndPoint ipEndPoint, int id, float timeStamp)
    {
        this.timeStamp = timeStamp;
        this.id = id;
        this.ipEndPoint = ipEndPoint;
    }
}

public class NetworkManager : MonoBehaviourSingleton<NetworkManager>, IReceiveData
{
    [SerializeField] private TextMeshProUGUI clientsTxt;

    public IPAddress ipAddress { get; private set; }

    public int port { get; private set; }

    public bool isServer { get; private set; }

    public int TimeOut = 30;

    public Action<byte[]> OnReceiveEvent;

    private UdpConnection connection;

    private readonly Dictionary<int, Client> clients = new();
    private readonly Dictionary<IPEndPoint, int> ipToId = new();
    private List<int> ids = new();

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

    void AddClient(IPEndPoint ip)
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
        }
    }

    void RemoveClient(IPEndPoint ip)
    {
        if (ipToId.ContainsKey(ip))
        {
            Debug.Log("Removing client: " + ip.Address);
            clients.Remove(ipToId[ip]);
        }
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
                    
                    foreach (int id in newIds)
                        ids.Add(id);
                }
                break;

            case MessageType.Console:
                OnReceiveEvent?.Invoke(data);
                break;

            case MessageType.Position:
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

    void Update()
    {
        // Flush the data in main thread
        if (connection != null)
            connection.FlushReceiveData();
        
        clientsTxt.text = "Clients: " + ids.Count;
    }
}