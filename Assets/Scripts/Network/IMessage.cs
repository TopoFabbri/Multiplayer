namespace Network
{
    public interface IMessage<T>
    {
        public MessageType GetMessageType();
        public byte[] Serialize();
        public T Deserialize(byte[] message);
    }
}