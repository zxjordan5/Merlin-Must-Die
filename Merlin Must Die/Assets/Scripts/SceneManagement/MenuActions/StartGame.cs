using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "StartGame", menuName = "MenuAction/StartGame")]
public class StartGame : MenuAction
{
    public override void Action()
    {
        GlobalSceneManager.Instance.StartGame();
    }
}
