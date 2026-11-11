using UnityEngine;

/// <summary>
/// A global singleton that holds all level path values. Referenced by any UI element or game event that needs to change scenes.
/// Automatically instantiates itself when referenced.
/// </summary>
[CreateAssetMenu(fileName = "LevelScenePaths", menuName = "Scriptable Objects/LevelScenePaths")]
public class LevelScenePaths : ScriptableObject
{
    public const string MainMenuScene = "Assets/Scenes/Menus/TempMainMenu.unity";
    public const string GameOverScene = "Assets/Scenes/Menus/TempGameOverScene.unity";
    public const string LevelOne = "Assets/Scenes/Playtests/pt-10-30.unity";
    public const string WinScene = "Assets/Scenes/Menus/TempWinScreen.unity";
}
