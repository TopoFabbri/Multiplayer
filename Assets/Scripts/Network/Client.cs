using System.Collections.Generic;
using System.Net;

namespace Network
{
    public class Client
    {
        public float timeStamp;
        public int id;
        public IPEndPoint ipEndPoint;
        
        private List<int> clientIds = new();
        
        public float Ms { get; set; }

        public Client(IPEndPoint ipEndPoint, int id, float timeStamp)
        {
            Ms = 0f;
            this.timeStamp = timeStamp;
            this.id = id;
            this.ipEndPoint = ipEndPoint;
        }

        
        public void UpdateList(List<int> clientIds)
        {
            this.clientIds = clientIds;
        }
    }
}