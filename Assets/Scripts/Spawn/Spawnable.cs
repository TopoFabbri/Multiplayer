using UnityEngine;

namespace Spawn
{
    public abstract class Spawnable : MonoBehaviour
    {
        public int id;

        public abstract Spawnable Spawn(int id);
    }
}