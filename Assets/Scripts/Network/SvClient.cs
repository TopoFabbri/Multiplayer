using System.Net;
using Network.MessageTypes;

namespace Network
{
    public class SvClient : Client
    {
        public SvClient(IPEndPoint ipEndPoint, int id, float timeStamp) : base(ipEndPoint, id, timeStamp)
        {
        }
        
        public override void SendToServer(byte[] message)
        {
            NetworkManager.Instance.server.HandleMessage(message, ipEndPoint);
        }

        protected override void HandlePing(byte[] message)
        {
        }
        
        protected override void HandleConsole(byte[] message)
        {
        }
    }
}