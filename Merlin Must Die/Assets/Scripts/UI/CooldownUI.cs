using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CooldownUI : MonoBehaviour
{

    /// <summary>
    /// Vertical layout group that will contain the spellbar UI elements
    /// </summary>
    [SerializeField]
    private VerticalLayoutGroup _cooldownUIArea;

    [SerializeField]
    Vector2 _cooldownBarOffset;


    //No longer needed -> not displaying UI
    //[SerializeField]
    //private GameObject _comboBar;

    //private TMP_Text _comboText;

    //[SerializeField]
    //private Image _comboBarImage;
    

    /// <summary>
    /// Filled image that can have it's fillAmount changed to match the attached spells cooldown
    /// </summary>
    [SerializeField]
    private GameObject _cooldownBar;

    /// <summary>
    /// Container for every current active spellbar UI 
    /// </summary>
    private List<Image> _activeCooldownBars = new List<Image>();
    private List<Color> _activeCooldownColors = new List<Color>();

    private void Awake()
    {

    }


    /// <summary>
    /// Creates a progress bar UI element using the spells information
    /// </summary>
    /// <param name="spell">Spell name and cooldown will be connected to the progress bar</param>
    public void AddSpell(Spell spell)
    {
        GameObject newBar = Instantiate(_cooldownBar, _cooldownUIArea.transform.position, _cooldownUIArea.transform.rotation);
        _activeCooldownBars.Add(newBar.GetComponent<Image>());
        _activeCooldownColors.Add(spell.spellColor);

        newBar.transform.SetParent(_cooldownUIArea.transform.Find(spell.SpellName + "UI"), false);
        newBar.transform.SetAsFirstSibling();
        RectTransform barRect = newBar.GetComponent<RectTransform>();
        barRect.anchoredPosition = _cooldownBarOffset;
        barRect.sizeDelta = new Vector2(_cooldownUIArea.GetComponent<RectTransform>().rect.width * 0.9f, barRect.sizeDelta.y);
    }

    /// <summary>
    /// Change spellbar fill percentage to match the associated spells cooldown
    /// </summary>
    /// <param name="spellIndex">Order of the spell to change the cooldown of</param>
    /// <param name="cooldownPercentage"></param>
    public void UpdateSpellBar(int spellIndex, float cooldownPercentage)
    {
        _activeCooldownBars[spellIndex].fillAmount = cooldownPercentage / 100;
        Color spellbarColor = Color.black + (_activeCooldownColors[spellIndex] * (cooldownPercentage / 100));
        spellbarColor.a = 0.5f;


        _activeCooldownBars[spellIndex].color = spellbarColor;
    }

    //public void UpdateComboText(int combo)
    //{
    //    _comboText.text = "Combo: " + combo;
    //}
    ///// <summary>
    ///// Updates the cooldown bar to count down, indicating when the cooldown will reset
    ///// </summary>
    //public void UpdateComboBar(float timeUntilReset, float resetTime)
    //{
    //    _comboBarImage.fillAmount = 1 - timeUntilReset / resetTime;
    //}
}