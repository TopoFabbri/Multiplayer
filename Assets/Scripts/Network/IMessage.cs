using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Net;

public enum MessageType
{
    HandShake = -1,
    Console = 0,
    Position = 1
}

public interface IMessage<T>
{
    public MessageType GetMessageType();
    public byte[] Serialize();
    public T Deserialize(byte[] message);
}

public class NetHandShake : IMessage<List<int>>
{
    private readonly List<int> data = new();

    public List<int> Deserialize(byte[] message)
    {
        List<int> outData = new();

        for (int i = 4; i < message.Length; i += 4)
        {
            byte[] curInt = {message[i], message[i + 1], message[i + 2], message[i + 3]};
            outData.Add(BitConverter.ToInt32(curInt, 0));
        }

        return outData;
    }

    public MessageType GetMessageType()
    {
        return MessageType.HandShake;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));

        foreach (int i in data)
            outData.AddRange(BitConverter.GetBytes(i));

        return outData.ToArray();
    }
    
    public void Add(int id)
    {
        data.Add(id);
    }
}

public class NetConsole : IMessage<string>
{
    public string data;

    public string Deserialize(byte[] message)
    {
        string outData = "";

        for (int i = 4; i < message.Length; i++)
            outData += (char)message[i];

        return outData;
    }

    public MessageType GetMessageType()
    {
        return MessageType.Console;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));

        foreach (char letter in data)
            outData.Add((byte)letter);

        return outData.ToArray();
    }
}

public class NetVector3 : IMessage<UnityEngine.Vector3>
{
    private static ulong lastMsgID = 0;
    private Vector3 data;

    public NetVector3(Vector3 data)
    {
        this.data = data;
    }

    public Vector3 Deserialize(byte[] message)
    {
        Vector3 outData;

        outData.x = BitConverter.ToSingle(message, 8);
        outData.y = BitConverter.ToSingle(message, 12);
        outData.z = BitConverter.ToSingle(message, 16);

        return outData;
    }

    public MessageType GetMessageType()
    {
        return MessageType.Position;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes(lastMsgID++));
        outData.AddRange(BitConverter.GetBytes(data.x));
        outData.AddRange(BitConverter.GetBytes(data.y));
        outData.AddRange(BitConverter.GetBytes(data.z));

        return outData.ToArray();
    }

    //Dictionary<Client,Dictionary<msgType,int>>
}