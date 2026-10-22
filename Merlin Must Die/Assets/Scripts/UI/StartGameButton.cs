using UnityEngine;

/// <summary>
/// EXAMPLE class for a button that when clicked, loads a scene
/// </summary>
public class StartGameButton : MonoBehaviour
{
    LevelScenePaths _levelScenePathsInstance;

    private void Awake()
    {
        // MUST create an instance of the LevelScenePaths ScriptableObject to access its data
        _levelScenePathsInstance = ScriptableObject.CreateInstance<LevelScenePaths>();
    }

    public void OnClick()
    {
        GlobalSceneManager.Instance.ChangeScene(_levelScenePathsInstance.LevelOne);
    }
}
