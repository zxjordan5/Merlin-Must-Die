using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalSceneManager : MonoBehaviour
{
    public static GlobalSceneManager Instance { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scene changes
    }


    /// <summary>
    /// Initialize the global instance
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitializeGlobalInstance()
    {
        GameObject prefab = Resources.Load<GameObject>("Managers/GlobalSceneManager");

        if (prefab != null)
        {
            Instantiate(prefab);
        }
        else
        {
            Debug.LogError("GlobalSceneManager prefab not found in Resources folder!");
        }
    }

    /// <summary>
    /// Loads a scene from a given scene path
    /// </summary>
    /// <param name="scenePath">Filepath to the scene to load</param>
    public void ChangeScene(string scenePath)
    {
        SceneManager.LoadScene(scenePath);
    }
}
