using Network;
using Network.MessageTypes;
using Players;
using UnityEngine;
using Utils;

namespace Spawn
{
    public class Spawner : MonoBehaviourSingleton<Spawner>
    {
        [SerializeField] private Spawnable player;
        [SerializeField] private Spawnable enemy;

        private bool spawnedPlayer;
        private int spawnedCounter;

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
            if (spawnedPlayer) return;
            
            NetSpawnRequest spawnRequest = new();
            
            NetworkManager.Instance.SendToServer(spawnRequest.Serialize());
        }

        public void Spawn(int id)
        {
            if (!spawnedPlayer)
            {
                for (int i = 0; i < id; i++)
                {
                    EnemyManager.Instance.SpawnEnemy(enemy, i);
                }
                
                player.Spawn(id);
            }
            else
            {
                EnemyManager.Instance.SpawnEnemy(enemy, id);
            }
            
            spawnedPlayer = true;
        }
    }
}