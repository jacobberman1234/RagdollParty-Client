using InexperiencedDeveloper.Utils.Log;
using Unity.Netcode;
using UnityEngine;

namespace InexperiencedDeveloper.Network.Core
{
    public class NetworkSingleton<T> : NetworkBehaviour where T : Component
    {
        protected static T instance;
        public static T Instance
        {
            get
            {
                if(instance == null)
                {
                    DebugLogger.Log($"Looking for singleton of type {typeof(T)}");
                    instance = FindObjectOfType<T>();

                    if(instance == null)
                    {
                        DebugLogger.LogWarning($"Couldn't find singleton of {typeof(T)}. Creating...");
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
                DebugLogger.LogWarning($"Duplicate singleton of {typeof(T)}. Destroying...");
                Destroy(gameObject);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (instance == this)
                instance = null;
        }
    }
}

