using Network;
using UnityEngine;

namespace Players
{
    public class EnemyController : Spawnable
    {
        public override void Spawn(int id)
        {
            this.id = id;
        
            Instantiate(this, Vector3.zero, Quaternion.identity);
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