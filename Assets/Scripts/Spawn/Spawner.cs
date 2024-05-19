using Network;
using Network.MessageTypes;
using UnityEngine;
using Utils;

namespace Spawn
{
    public class Spawner : MonoBehaviourSingleton<Spawner>
    {
        [SerializeField] private Spawnable player;
        [SerializeField] private Spawnable enemy;

        private bool spawnedPlayer;

        private void OnEnable()
        {
            Client.Connected += OnConnected;
        }
        
        private void OnDisable()
        {
            Client.Connected -= OnConnected;
        }

        private void OnConnected()
        {
            NetSpawnRequest spawnRequest = new();
        
            if (NetworkManager.Instance.server != null)
                NetworkManager.Instance.server.HandleMessage(spawnRequest.Serialize(false), NetworkManager.Instance.server.svClient.ipEndPoint);
            else
                NetworkManager.Instance.SendToServer(spawnRequest.Serialize(false));
        }

        public void Spawn(int id)
        {
            if (!spawnedPlayer)
                player.Spawn(id);
            else
                enemy.Spawn(id);
            
            spawnedPlayer = true;
        }
    }
}