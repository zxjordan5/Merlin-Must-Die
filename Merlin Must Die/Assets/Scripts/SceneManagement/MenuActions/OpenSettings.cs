using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "OpenSettings", menuName = "MenuAction/OpenSettings")]
public class OpenSettings : MenuAction
{
    public override void Action()
    {
        GlobalSceneManager.Instance.OpenSettings();
    }
}
