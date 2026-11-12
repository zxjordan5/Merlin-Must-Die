using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.VisualScripting;

/// <summary>
/// Grimoire Class
/// Contains the full grimoire and a list of active spells in the level
/// Reads player input and finds the matching spell code
/// Shows cooldowns for spells
/// </summary>
public class Grimoire : MonoBehaviour
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }


    // TODO: MOVE THESE TO A SCRIPTABLE OBJECT
    private List<Spell> _allSpells;
    private List<Spell> _learnedSpells;

    /// <summary>
    /// List of spells that the player currently has available within the level
    /// </summary>
    [SerializeField]
    public List<Spell> activeSpells;

    private List<Spell> _instantiatedActiveSpells = new List<Spell>();

    /// <summary>
    /// The currently prepared spell of the player. If this is null, the player has no spell prepared
    /// </summary>
    private Spell _preparedSpell;

    /// <summary>
    /// Tracks longest spell code to reset after that limit is reached
    /// </summary>
    private int _maxSpellLength = 0;


    /// <summary>
    /// Current sequence of inputs the player has entered
    /// </summary>
    private List<Direction> _playerInputSequence = new List<Direction>();

    /// <summary>
    /// The current player in the scene
    /// </summary>
    private Player _player;

    //***** UI FIELDS *****
    #region UI
    // The Spell Input GO with 4 cardinal direction arrow sprites as children
    [SerializeField]
    private HorizontalLayoutGroup _spellIconArea;
    [SerializeField]
    private GameObject _spellInputIcon;
    [SerializeField]
    private List<GameObject> _arrowIcons;
    [SerializeField]
    private CooldownUI _cooldownUI;
    [SerializeField]
    private Image _preparedSpellIcon;
    [SerializeField]
    private Image _spellBlockedIcon;
    [SerializeField]
    private ParticleSystem _preparedSpellParticles;
    private ParticleSystem _activeIndicatorParticles;

    private bool _collapsingIcons;
    private float _initialIconSpacing;

    private List<GameObject> _createdSpellIcons = new List<GameObject>();


    #endregion //UI END

    //***** COMBO FIELDS *****
    private int _comboCounter;
    private float _comboTimer;
    private float _comboGapTime = 1.5f;

    //Excalibur
    [SerializeField] private Excalibur _excalibur;

    [SerializeField] private GameObject grimoireContents;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // get all spells from spell class; sort into .learned and .active
        _allSpells = new List<Spell>();
        _preparedSpell = null;
        _player = FindAnyObjectByType<Player>();

        GetGrimoireContents();

        // The original spell input icon prefab is hidden; used for instantiating only
        _spellInputIcon.SetActive(false);
        //Get default spacing for future resets
        _initialIconSpacing = _spellIconArea.spacing;

        InstantiateSpells();
    }

    void Update()
    {
        UpdateComboTimer();

        //Update cooldown bar UI
        for(int i = 0; i < _instantiatedActiveSpells.Count; i++)
        {
            _cooldownUI.UpdateSpellBar(i, _instantiatedActiveSpells[i].CooldownPercent);
        }

        //Collapse spell icons (when a spell is cast)
        if (_collapsingIcons)
        {
            _spellIconArea.spacing -= Time.deltaTime * _initialIconSpacing * 10; //Should take 1/10th of a second
            
            //Once the spacing is zero, the icons are collapsed. Stop collapsing them and clear the icons.
            if (_spellIconArea.spacing <= 0)
            {
                _collapsingIcons = false;
                if(_preparedSpell)
                    _preparedSpellIcon.enabled = true;
                _spellIconArea.spacing = _initialIconSpacing;
                ResetInputIconsUI();
            }
        }

        if (_preparedSpell && _preparedSpell.OnCooldown)
            _spellBlockedIcon.enabled = true;
        else
            _spellBlockedIcon.enabled = false;
    }

    /// <summary>
    /// Gets all grimoire contents and displays it in the UI
    /// </summary>
    /// <returns></returns>
    void GetGrimoireContents()
    {
        // A list of each spell with spell icons stored in a list
        List<List<GameObject>> sprites = new List<List<GameObject>>();

        // Cycle through every spell for this level
        for (int i = 0; i < activeSpells.Count; i++)
        {
            // A list of all sprites for ONE spell
            List<GameObject> spellSprites = new List<GameObject>();

            // Spell Icon (GO) - the actual icon is added to the sprites list later
            spellSprites.Add(activeSpells[i].gameObject);

            // Spell Code icons
            List<Direction> activeSpellCode = activeSpells[i].spellCode;
            for (int j = 0; j < activeSpellCode.Count; j++)
            {
                switch (activeSpellCode[j])
                {
                    case Direction.Up:
                        spellSprites.Add(_arrowIcons[0]);
                        break;
                    case Direction.Down:
                        spellSprites.Add(_arrowIcons[1]);
                        break;
                    case Direction.Left:
                        spellSprites.Add(_arrowIcons[2]);
                        break;
                    case Direction.Right:
                        spellSprites.Add(_arrowIcons[3]);
                        break;
                }
            }
            sprites.Add(spellSprites);
        }

        // Adding contents to the grimoireContents GO

        // Loops through each spell (a list of spell's sprites on the UI)
        for (int i = 0; i < sprites.Count; i++)
        {
            int index = sprites[i].Count;

            GameObject spellUI = new GameObject(activeSpells[i].SpellName + "UI");
            UnityEngine.UI.LayoutElement spellUILayout = spellUI.AddComponent<UnityEngine.UI.LayoutElement>();

            spellUI.transform.SetParent(grimoireContents.transform, false); 
            //spellUILayout.preferredWidth = 45;
            //spellUILayout.preferredHeight = 45;
            spellUILayout.minHeight = 45;
            //spellUILayout.minWidth = 45;

            // Spell List contains all icons within a spell (name icon + directions)
            //GameObject spellBlock = new GameObject("SpellUI" + i, typeof()

            GameObject spellList = new GameObject("SpellList" + i, typeof(UnityEngine.UI.HorizontalLayoutGroup));
            UnityEngine.UI.HorizontalLayoutGroup xLayout = spellList.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
            xLayout.childAlignment = TextAnchor.MiddleLeft;
            xLayout.childControlHeight = true;
            xLayout.childControlWidth = true;
            xLayout.childForceExpandWidth = false;
            xLayout.childForceExpandHeight = false;
            xLayout.spacing = 20;
            spellList.transform.SetParent(spellUI.transform, false);

            // Puts all icons in the spell list component
            for (int j = 0; j < index; j++)
            {
             
                // Defining icons as part of the layout + sizing to fit in grimoire
                GameObject iconSprite = Instantiate(new GameObject("SpellSprites" + i + "_" + j), grimoireContents.transform);
                iconSprite.transform.SetParent(spellList.transform);
                Image grimoireSprite = iconSprite.AddComponent<Image>();
                UnityEngine.UI.LayoutElement layout = iconSprite.AddComponent<UnityEngine.UI.LayoutElement>();
                layout.preferredWidth = 45;
                layout.preferredHeight = 45;
                layout.minHeight = 45;
                layout.minWidth = 45;

                // The first icon (spell name icon)
                if (j == 0)
                {
                    grimoireSprite.sprite = activeSpells[i].spellIcon;
                    iconSprite.name = activeSpells[i].SpellName + " Grimoire Icon"; //Change name for future particle check
                    _createdSpellIcons.Add(iconSprite);
                }

                // All other icons (directional inputs)
                else
                {
                    grimoireSprite.sprite = sprites[i][j].GetComponent<SpriteRenderer>().sprite;
                }
            }
        }
    }

    public void InstantiateSpells()
    {
        foreach (Spell spell in activeSpells)
        {
            // Need to instantiate spells in order to track their timers
            _instantiatedActiveSpells.Add(Instantiate(spell, transform.position, Quaternion.identity));

            //Get the longest spell code
            if (spell.spellCode.Count > _maxSpellLength)
            {
                _maxSpellLength = spell.spellCode.Count;
            }

            //Add the spell to the cooldown UI element
            _cooldownUI.AddSpell(spell);
        }
    }

    /// <summary>
    /// Updates the player input sequence with the new input and checks for matches with active spell codes
    /// </summary>
    /// <param name="input">The direction of the new input</param>
    public void ProcessInput(Direction input)
    {
        if (_playerInputSequence.Count == _maxSpellLength + 1)
        {
            ResetInputSequence();
        }

        //Will only read input if a spell is not prepared
        if (!_preparedSpell)
        {
            // Makes the input icons appear above the player's head when a spell is not prepared
            _playerInputSequence.Add(input);

            HandleSpellInputIcon(input);
        }

        CheckForMatches();
    }

    public void CheckForMatches()
    {
        //Check if no spells match
        int spellMismatches = 0;

        foreach (Spell spell in _instantiatedActiveSpells)
        {
            if (IsMatch(spell))
            {
                PrepareSpell(spell);
                break; // Exit the loop after preparing a spell
            }
            // if all spells on cooldown, reset input sequence
            if (!IsSequenceValid(spell))
            {
                spellMismatches++;
            }
        }
        //Resets input when no spells match
        if (spellMismatches >= _instantiatedActiveSpells.Count)
        {
            ResetInputSequence();
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="spell"></param>
    /// <returns>True if the players input sequence matches the spells spellcode</returns>
    private bool IsMatch(Spell spell)
    {
        int codeLength = spell.spellCode.Count;
        //Don't check if spell can be cast if the code is longer already
        if (_playerInputSequence.Count < codeLength)
        {
            return false;
        }

        for (int i = 0; i < codeLength; i++)
        {
            // Compare the last 'codeLength' inputs in playerInputSequence with the spell's spellCode
            // Quit out early after any mismatches
            if (_playerInputSequence[i] != spell.spellCode[i])
            {
                return false;
            }
        }

        //Debug.Log("INPUT CORRECT, SPELL PREPARED");
        return true;
    }

    /// <summary>
    /// </summary>
    /// <param name="spell"></param>
    /// <returns>True if the current input sequence is contained within a spells full input sequence
    /// False as soon as the current input does not match the spell
    /// </returns>
    private bool IsSequenceValid(Spell spell)
    {
        //Sequence is invalid if it is longer than the current spell
        if (_playerInputSequence.Count > spell.spellCode.Count)
        {
            return false;
        }
        for (int i = 0; i < _playerInputSequence.Count; i++)
        {
            if (_playerInputSequence[i] != spell.spellCode[i])
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// Clear the current player input seqeunce
    /// </summary>
    public void ResetInputSequence()
    {
        _playerInputSequence.Clear();
        ResetInputIconsUI();
    }

    /// <summary>
    /// A debug function for printing the current inputted characters
    /// </summary>
    private void DebugInputString()
    {
        string output = "Input: ";
        foreach (Direction input in _playerInputSequence)
        {
            output += input + ", ";
        }
        Debug.Log(output);
    }

    /// <summary>
    /// Sets the prepared spell, plays the spell collapse animation, and updates the prepared spell icon
    /// Instantiates particles for the prepared spell in the grimoire
    /// </summary>
    public void PrepareSpell(Spell spell)
    {
        if (_activeIndicatorParticles)
            Destroy(_activeIndicatorParticles);

        _preparedSpell = spell;

        //Loop through the list of spell icons and check if they match the spell being prepared
        //If they do, instantiate the prepared spell particles at that position
        foreach (GameObject spellIcon in _createdSpellIcons)
        {
            if (spellIcon.name.Contains(spell.SpellName))
            {
                _activeIndicatorParticles = Instantiate(_preparedSpellParticles, spellIcon.transform);
                ParticleSystem.MainModule main = _activeIndicatorParticles.main;
                main.startColor = spell.spellColor; //Particles should match spell color
                break;
            }
        }

        CollapseSpellIcons();
        _preparedSpellIcon.sprite = spell.spellIcon;
    }

    /// <summary>
    /// Cast prepared spell and reset input sequence
    /// </summary>
    public void OnCast(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
           
            //Extra check for a prepared spell
            if (!_preparedSpell)
                CheckForMatches();

            if (_preparedSpell && !_preparedSpell.OnCooldown)
            {
                //(REMOVED to use spells hitting enemies instead)
                //Charges Excalibur based on spell strength
                //_excalibur.IncreaseCharge(_preparedSpell.ExcaliburCharge);

                _player.SpellCast();

                _preparedSpell.Cast(_player);

                ClearPreparedSpell();

                UpdateCombos();
            }
            //Prepared spell exists but is on cooldown, clear spell
            else
            {
                ClearPreparedSpell();
            }

            ResetInputSequence();
        }
    }
    
    public void ClearPreparedSpell()
    {
        _preparedSpell = null;
        _preparedSpellIcon.enabled = false;
        if (_activeIndicatorParticles)
            Destroy(_activeIndicatorParticles);
    }

    // === UI ===

    /// <summary>
    /// Determines the direction of the spell icon arrow based on the current input
    /// </summary>
    /// <param name="input"></param>
    public void HandleSpellInputIcon(Direction input)
    {
        if (_spellInputIcon.transform.parent.childCount <= _maxSpellLength)
        {
            // Gets the parent SpellInput object
            GameObject thisIcon = Instantiate(_spellInputIcon, _spellInputIcon.transform.parent.transform);
            thisIcon.SetActive(true);

            // Set arrow icons active false - invisible
            for (int i = 0; i < _arrowIcons.Count; i++)
            {
                _arrowIcons[i] = thisIcon.transform.GetChild(i).gameObject;
                _arrowIcons[i].SetActive(false);
            }
            // assuming these are in order -- check later
            switch (input)
            {
                case Direction.Up:
                    _arrowIcons[0].SetActive(true);
                    break;
                case Direction.Down:
                    _arrowIcons[1].SetActive(true);
                    break;
                case Direction.Left:
                    _arrowIcons[2].SetActive(true);
                    break;
                case Direction.Right:
                    _arrowIcons[3].SetActive(true);
                    break;
            }
        }
    }

    /// <summary>
    /// No input icons displayed over player's head
    /// Used in ResetInputSequence()
    /// </summary>
    public void ResetInputIconsUI()
    {
        for (int i = _spellInputIcon.transform.parent.childCount - 1; i > 0; i--)
        {
            if (_spellInputIcon.transform.parent.GetChild(i) != null)
            {
                Destroy(_spellInputIcon.transform.parent.GetChild(i).gameObject);
            }
        }
    }

    /// <summary>
    /// Setting _collapsingIcons to true will lower the spacing of the icon area in Update()
    /// </summary>
    public void CollapseSpellIcons()
    {
        _collapsingIcons = true;
    }

    // =============

    // **** COMBO METHODS ****
    #region Combos
    /// <summary>
    /// Increase the combo count and reset the combo cooldown
    /// </summary>
    private void UpdateCombos()
    {
        _comboCounter++;
        _comboTimer = 0;
        foreach(Spell spell in _instantiatedActiveSpells)
        {
            spell.ReduceCooldownPercent(_comboCounter);
        }
    }

    private void UpdateComboTimer()
    {
        if (_comboTimer > _comboGapTime)
        {
            _comboTimer = 0;
            _comboCounter = 0;
        }
        _comboTimer += Time.deltaTime;
    }
    #endregion

    // ***** INPUT METHODS *****
    #region Input Methods
    public void OnPrepareSpellUp(InputAction.CallbackContext context)
    {
        if (context.performed && _player.HasGear)
        {
            ProcessInput(Direction.Up);
        }
    }

    public void OnPrepareSpellDown(InputAction.CallbackContext context)
    {
        if (context.performed && _player.HasGear)
        {
            ProcessInput(Direction.Down);
        }
    }

    public void OnPrepareSpellLeft(InputAction.CallbackContext context)
    {
        if (context.performed && _player.HasGear)
        {
            ProcessInput(Direction.Left);
        }
    }

    public void OnPrepareSpellRight(InputAction.CallbackContext context)
    {
        if (context.performed && _player.HasGear)
        {
            ProcessInput(Direction.Right);
        }
    }
    #endregion
    // *************************
}