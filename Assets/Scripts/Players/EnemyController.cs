using Network;
using Spawn;
using UnityEngine;

namespace Players
{
    public class EnemyController : Spawnable
    {
        public override Spawnable Spawn(int id)
        {
            Spawnable instance = Instantiate(this, Vector3.zero + Vector3.right * id, Quaternion.identity);
            
            instance.id = id;
            
            return instance;
        }
    
        public void UpdatePosition(Vector3 position)
        {
            transform.position = position;
        }
    
        public void UpdateRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }
    }
}