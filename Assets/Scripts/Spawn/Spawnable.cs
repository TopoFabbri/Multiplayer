using UnityEngine;

namespace Network
{
    public abstract class Spawnable : MonoBehaviour
    {
        protected int id;

        public abstract void Spawn(int id);
    }
}