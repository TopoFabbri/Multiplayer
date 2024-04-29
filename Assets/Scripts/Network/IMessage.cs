public enum MessageType
{
    HandShake = -1,
    Console = 0,
    Position = 1,
    PingPong = 2
}

public interface IMessage<T>
{
    public MessageType GetMessageType();
    public byte[] Serialize();
    public T Deserialize(byte[] message);
}