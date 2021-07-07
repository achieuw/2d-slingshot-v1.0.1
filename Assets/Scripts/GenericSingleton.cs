using UnityEngine;

// <summary>
// Inherit from this base class to create a singleton
// e.g public class MyClassName : GenericSingleton<MyClassName> ()
// </summary>
public class GenericSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // Check to see if we're about to get destroyed
    static bool m_ShuttingDown = false;
    static object m_Lock = new Object();
    static T m_Instance;

    // Access singleton instance
    public static T Instance
    {
        get
        {
            if (m_ShuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed. Returning null.");
                return null;
            }

            lock (m_Lock)
            {
                if(m_Instance == null)
                {
                    // Search for existing instance
                    m_Instance = (T)FindObjectOfType(typeof(T));

                    if(m_Instance == null)
                    {
                        var singletonObject = new GameObject();
                        m_Instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";

                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return m_Instance;
            }    
        }
    }

    private void OnApplicationQuit()
    {
        m_ShuttingDown = true;
    }

    private void OnDestroy()
    {
        m_ShuttingDown = true;
    }
}
