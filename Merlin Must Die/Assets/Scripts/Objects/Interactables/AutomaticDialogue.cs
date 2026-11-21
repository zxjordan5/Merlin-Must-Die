using UnityEngine;
using System.Collections;

public class AutomaticDialogue : Interaction
{
    private ShowDialogue _dialogue;
    [SerializeField] private float _displayTime;
    //[SerializeField] private bool _skippable;


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

    public override void Behaviour()
    {
        StopAllCoroutines();
        _dialogue.Behaviour();
        StartCoroutine("NextDialogue", _displayTime);
    }
}
