namespace Network
{
    public abstract class Singleton<T> where T : new()
    {
        private T instance;
        
        public T Instance
        {
            get
            {
                if (instance == null)
                    instance = new T();
                
                return instance;
            }
        }
    }
}