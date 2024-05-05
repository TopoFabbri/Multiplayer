using System.Net;

namespace Network
{
    public class Client
    {
        public float ms;
        public float timeStamp;
        public int id;
        public IPEndPoint ipEndPoint;

        public Client(IPEndPoint ipEndPoint, int id, float timeStamp)
        {
            ms = 0f;
            this.timeStamp = timeStamp;
            this.id = id;
            this.ipEndPoint = ipEndPoint;
        }
        
        public void ResetMs()
        {
            ms = 0f;
        }
        
        public void SetMs(float ms)
        {
            this.ms = ms;
        }
    }
}