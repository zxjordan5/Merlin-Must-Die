using UnityEngine;

/// <summary>
/// Scriptable Object that contains text information for an NPC
/// </summary>
[CreateAssetMenu(fileName = "DialogueAsset", menuName = "DialogueAsset")]
public class DialogueAsset : ScriptableObject
{
    [TextArea]
    public string[] dialogue;

    [TextArea]
    public string[] tags = new string[10];

    public int DialogueCount
    {
        get { return dialogue.Length; }
    }
}
