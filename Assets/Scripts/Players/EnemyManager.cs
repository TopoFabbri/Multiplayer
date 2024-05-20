using System.Collections.Generic;
using Network;
using UnityEngine;
using Utils;

namespace Players
{
    public class EnemyManager : MonoBehaviourSingleton<EnemyManager>
    {
        private readonly Dictionary<int, EnemyController> enemiesById = new();

        public void SpawnEnemy(Spawnable enemy, int id)
        {
            EnemyController instance = enemy.Spawn(id) as EnemyController;
            
            enemiesById.Add(id, instance);
            
            Debug.Log("Spawned enemy: " + id);
        }
        
        public void RemoveEnemy(int id)
        {
            enemiesById.Remove(id);
        }
        
        public void UpdatePosition(int id, Vector3 position)
        {
            if (!enemiesById.TryGetValue(id, out EnemyController enemy))
                return;
            
            enemy.UpdatePosition(position);
        }
        
        public void UpdateRotation(int id, Quaternion rotation)
        {
            if (!enemiesById.TryGetValue(id, out EnemyController enemy)) return;
            
            enemy.UpdateRotation(rotation);
        }
    }
}