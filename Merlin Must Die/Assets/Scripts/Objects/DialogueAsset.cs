using UnityEngine;

/// <summary>
/// Scriptable Object that contains text information for an NPC
/// </summary>
[CreateAssetMenu(fileName = "DialogueAsset", menuName = "DialogueAsset")]
public class DialogueAsset : ScriptableObject
{
    [TextArea]
    public string[] dialogue;

    public int DialogueCount
    {
        get { return dialogue.Length; }
    }
}
