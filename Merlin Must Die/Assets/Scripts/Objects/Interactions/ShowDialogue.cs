using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowDialogue : Interaction
{
    [SerializeField] DialogueAsset mainText;
    [SerializeField] DialogueAsset finishedText;

    private Sprite _grimoireTextBG;


    private GameObject _npcTextArea;
    private Image _moreTextIndicator;
    private TMP_Text _npcText;

    private Player _player;

    private int _dialogueCounter = 0;
    public int DialogueSeen { get { return _dialogueCounter; } }
    private bool _allDialogueSeen = false;
    public bool AllDialogueSeen { get { return _allDialogueSeen; } }
    private static float _dialogueDisplayTime = 8f;

    public void Awake()
    {
        _player = FindAnyObjectByType<Player>();
        _npcTextArea = GameObject.Find("NPCTextArea");
        _moreTextIndicator = _npcTextArea.transform.Find("Indicator").GetComponent<Image>();
        _npcText = _npcTextArea.GetComponentInChildren<TMP_Text>();

        _grimoireTextBG = Resources.Load<Sprite>("Sprites/UI/Grimoire-Background");
    }

    /// <summary>
    /// Load the DialogueAsset text into the text area based on how much has been scene
    /// Starts a coroutine to clear the dialogue
    /// </summary>
    public override void Behaviour()
    {
        StopAllCoroutines();

        _npcTextArea.transform.position = new Vector2(transform.position.x, transform.position.y - 1.2f);
        _player.ReadingDialogue = true;

        //Check if all dialogue has been seen. Reset counter to play it again
        if (_dialogueCounter >= mainText.DialogueCount)
        {
            _allDialogueSeen = true;
            _dialogueCounter = 0;
            EndDialogue();
            return; //Quit early when using EndDialogue() to prevent dialogue counter from incrementing
        }
        //Disable more dialogue indactor when the last text has been loaded
        if(_dialogueCounter == mainText.DialogueCount - 1)
            _moreTextIndicator.enabled = false;
        else
            _moreTextIndicator.enabled = true;

        //If all dialogue has been seen and there is repeated dialogue at the end, play that
        if (_allDialogueSeen && finishedText)
        {
            LoadDialogue(finishedText, 0);
        }
        //Otherwise display text in order
        else
        {
            LoadDialogue(mainText, _dialogueCounter);
        }

        _dialogueCounter++;


        StartCoroutine("ClearAfterTime", _dialogueDisplayTime);
    }

    /// <summary>
    /// Hide dialogue area
    /// </summary>
    public void EndDialogue()
    {
        if(_npcTextArea)
            _npcTextArea.transform.Translate(new Vector3(1000, 1000, 0));

        _player.ReadingDialogue = false;
    }
    
    /// <summary>
    /// Clears the dialogue in half a second after leaving the dialogue radius
    /// </summary>
    public override void Release()
    {
        if(isActiveAndEnabled)
            StartCoroutine("ClearAfterTime", .6f);
    }

    /// <summary>
    /// Loads a line of dialogue from a DialogueAsset, updating the text display
    /// Searches tags on the dialogue for special formatting properties
    /// </summary>
    /// <param name="asset">DialogueAsset to load</param>
    /// <param name="count">Which dialogue line to load</param>
    public void LoadDialogue(DialogueAsset asset, int count)
    {
        string text = asset.dialogue[count];

        Image textBG = _npcTextArea.GetComponent<Image>();
        textBG.sprite = null;
        textBG.color = new Color(0, 0, 0, 0.325f);
        _npcText.color = Color.white;

        //Grimoire tag - change background to be the grimoire
        if (asset.tags[count].Contains("#grimoire"))
        {
            textBG.sprite = _grimoireTextBG;
            textBG.color = new Color(1f, 1f, 1f, 1f);
            _npcText.color = Color.black;
        }

        _npcText.text = text;
    }
    IEnumerator ClearAfterTime(float clearTime)
    {
        yield return new WaitForSeconds(clearTime);
        EndDialogue();
    }
}
