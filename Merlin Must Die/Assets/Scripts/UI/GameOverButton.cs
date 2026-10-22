using UnityEngine;

public class GameOverButton : MonoBehaviour
{
    LevelScenePaths _levelScenePathsInstance;

    private void Awake()
    {
        // MUST create an instance of the LevelScenePaths ScriptableObject to access its data
        _levelScenePathsInstance = ScriptableObject.CreateInstance<LevelScenePaths>();
    }

    public void OnClick()
    {
        GlobalSceneManager.Instance.ChangeScene(_levelScenePathsInstance.MainMenuScene);
    }
}
