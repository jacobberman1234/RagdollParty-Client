using UnityEngine;

namespace InexperiencedDeveloper.Core
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        protected static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    Utils.Log.DebugLogger.Log($"Looking for singleton of type {typeof(T)}");
                    instance = FindObjectOfType<T>();

                    if (instance == null)
                    {
                        Utils.Log.DebugLogger.LogWarning($"Couldn't find singleton of {typeof(T)}. Creating...");
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        instance = obj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Utils.Log.DebugLogger.LogWarning($"Duplicate singleton of {typeof(T)}. Destroying...");
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }
    }
}

