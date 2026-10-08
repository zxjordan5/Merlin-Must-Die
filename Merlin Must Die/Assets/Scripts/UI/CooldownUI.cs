using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class CooldownUI : MonoBehaviour
{

    /// <summary>
    /// Vertical layout group that will contain the spellbar UI elements
    /// </summary>
    [SerializeField]
    private VerticalLayoutGroup _grimoireUIArea;

    [SerializeField]
    private GameObject _comboBar;
    private Image _comboBarImage;
    private TMP_Text _comboText;

    /// <summary>
    /// Filled image that can have it's fillAmount changed to match the attached spells cooldown
    /// </summary>
    [SerializeField]
    private GameObject _spellbar;

    /// <summary>
    /// Container for every current active spellbar UI 
    /// </summary>
    private List<Image> _currentSpellbars = new List<Image>();

    private void Awake()
    {
        _comboText = _comboBar.GetComponentInChildren<TMP_Text>();
        _comboBarImage = _comboBar.GetComponent<Image>();
    }


    /// <summary>
    /// Creates a progress bar UI element using the spells information
    /// </summary>
    /// <param name="spell">Spell name and cooldown will be connected to the progress bar</param>
    public void AddSpell(Spell spell)
    {
        GameObject newBar = Instantiate(_spellbar, _grimoireUIArea.transform);
        newBar.GetComponentInChildren<TMP_Text>().text = spell.SpellName;
        newBar.name = spell.SpellName + " bar";

        _currentSpellbars.Add(newBar.GetComponent<Image>());
    }

    /// <summary>
    /// Change spellbar fill percentage to match the associated spells cooldown
    /// </summary>
    /// <param name="spellIndex">Order of the spell to change the cooldown of</param>
    /// <param name="cooldownPercentage"></param>
    public void UpdateSpellBar(int spellIndex, float cooldownPercentage)
    {
        _currentSpellbars[spellIndex].fillAmount = cooldownPercentage / 100;
    }
    public void UpdateComboText(int combo)
    {
        _comboText.text = "Combo: " + combo;
    }
    public void UpdateComboBar(float timeUntilReset, float resetTime)
    {
        _comboBarImage.fillAmount = 1 - timeUntilReset / resetTime;
    }
}