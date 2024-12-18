using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;
    protected static readonly object lockObject = new object();
    protected static bool applicationIsQuitting = false;

    private void Start()
    {
        DontDestroyOnLoad(Instance.gameObject);
    }

    public static T Instance
    {
        get
        {
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
}