using UnityEngine;

public class Level_Manager : MonoBehaviour
{
    [SerializeField] public GameObject EnemyParent;
    //[SerializeField] public GameObject LevelEndBox;

    private void Update()
    {
        
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // Checks if all enemys are dead before moving onto the next level
        if (EnemyParent.transform.childCount == 0)
        {
            GlobalSceneManager.Instance.ChangeScene(LevelScenePaths.WinScene);
        }

    }

    public void OnClick()
    {
        switch (GlobalSceneManager.Instance.CurrentScene)
        {
            // Main menu is the default
            default: 
                GlobalSceneManager.Instance.ChangeScene(LevelScenePaths.LevelOne); //_levelScenePathsInstance.LevelOne
                break;
            // GameOver & Win screens both lead to the Main menu
            case LevelScenePaths.GameOverScene:
            case LevelScenePaths.WinScene:
                GlobalSceneManager.Instance.ChangeScene(LevelScenePaths.MainMenuScene);
                break;
        }
        
    }
}


/// <summary>
/// EXAMPLE class for a button that when clicked, loads a scene
/// Keeping it here so I can delete the script without fear
/// </summary>
/*public class StartGameButton : MonoBehaviour
{
    LevelScenePaths _levelScenePathsInstance;

    public void OnClick()
    {
        // MUST create an instance of the LevelScenePaths ScriptableObject to access its data
        _levelScenePathsInstance = ScriptableObject.CreateInstance<LevelScenePaths>();
        GlobalSceneManager.Instance.ChangeScene(LevelScenePaths.LevelOne);
    }
}*/
