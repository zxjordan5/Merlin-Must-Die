using UnityEngine;
using System.Collections;

public class AutomaticDialogue : MonoBehaviour
{
    private ShowDialogue _dialogue;
    [SerializeField] private float _displayTime;

    public void Awake()
    {
        _dialogue = GetComponent<ShowDialogue>();

        StartCoroutine("NextDialogue", _displayTime);
    }

    public IEnumerator NextDialogue(float displayTime)
    {
        yield return new WaitForSeconds(displayTime);
        if (!_dialogue.AllDialogueSeen)
        {
            _dialogue.Behaviour();
            StartCoroutine("NextDialogue", _displayTime);
        }
            
    }
}
