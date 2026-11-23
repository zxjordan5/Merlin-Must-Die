using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "QuitGame", menuName = "MenuAction/QuitGame")]
public class QuitGame : MenuAction
{
    public override void Action()
    {
        GlobalSceneManager.Instance.QuitGame();
    }
}
