using UnityEngine;
using UnityEngine.SceneManagement;

public class VolatilitySingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;
    protected static readonly object lockObject = new object();
    protected static bool applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed on application quit." +
                    " Won't create again - returning null.");
                return null;
            }

            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";
                    }
                }

                return instance;
            }
        }
    }

    private void OnDestroy()
    {
        lock (lockObject)
        {
            if (instance != null)
            {
                instance = null;
            }
        }
    }
}