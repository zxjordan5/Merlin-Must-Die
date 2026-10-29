using UnityEngine;

/// <summary>
/// A global singleton that holds all level path values. Referenced by any UI element or game event that needs to change scenes.
/// Automatically instantiates itself when referenced.
/// </summary>
[CreateAssetMenu(fileName = "LevelScenePaths", menuName = "Scriptable Objects/LevelScenePaths")]
public class LevelScenePaths : ScriptableObject
{
    public string MainMenuScene = "Assets/Scenes/SceneTransitionTesting/TempMainMenu.unity";
    public string GameOverScene = "Assets/Scenes/SceneTransitionTesting/TempGameOverScene.unity";
    public string LevelOne = "Assets/Scenes/pt-10-30.unity";
}
