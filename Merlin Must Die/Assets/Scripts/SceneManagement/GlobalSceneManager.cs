using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalSceneManager : MonoBehaviour
{
    public static GlobalSceneManager Instance { get; private set; }
    public string CurrentScene { get; private set; }

    public bool InMenu { get { return CurrentScene.Contains("Menu"); } }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene changes
            CurrentScene = SceneManager.GetActiveScene().path;
            return;
        }

        CurrentScene = SceneManager.GetActiveScene().path;
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
        CurrentScene = scenePath;
        SceneManager.LoadScene(scenePath);
    }

    /// <summary>
    /// Start game by loading the first level
    /// 
    /// TODO (FUTURE)
    /// Save the players furthest level and map progress for continuing
    /// </summary>
    public void StartGame()
    {
        CurrentScene = LevelScenePaths.LevelOne;
        SceneManager.LoadScene(LevelScenePaths.LevelOne);
    }

    /// <summary>
    /// Closes the game 
    /// 
    /// TODO (FUTURE)
    /// Write current level and stats to a file
    /// Also export game metrics
    /// </summary>
    public void QuitGame()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
            EditorApplication.Beep();
        }
        Application.Quit();
    }

    public void OpenSettings()
    {
        CurrentScene = LevelScenePaths.SettingsScene;
        SceneManager.LoadScene(LevelScenePaths.SettingsScene);
    }
}
