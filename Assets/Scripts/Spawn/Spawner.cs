using UnityEngine;
using Utils;

namespace Network
{
    public class Spawner : MonoBehaviourSingleton<Spawner>
    {
        [SerializeField] private Spawnable player;
        [SerializeField] private Spawnable enemy;

        private bool spawnedPlayer;
        
        public void Spawn(int id)
        {
            if (!spawnedPlayer)
                player.Spawn(id);
            else
                enemy.Spawn(id);
        }
    }
}