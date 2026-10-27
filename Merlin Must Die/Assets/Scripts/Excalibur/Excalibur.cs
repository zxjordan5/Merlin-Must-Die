using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Excalibur
/// Contains selected Excalibur abilities and casts them
/// Recieves charge from the Grimoire and updates the UI
/// </summary>
public class Excalibur : MonoBehaviour
{

    [SerializeField] private ExcaliburAbility _abilityOne;
    [SerializeField] private ExcaliburAbility _abilityTwo;

    //UI
    private Image _excaliburBar;

    private float _charge = 0;

    /// <summary>
    /// Charge property. Keeps charge bounded between 0 and 100.
    /// </summary>
    public float Charge
    {
        get { return _charge; }
        set {
            if (value < 0)
                _charge = 0;
            else if (value > 100)
                _charge = 100;
            else
                _charge = value;
        }
    }

    //Fields for charge decreasing
    [SerializeField] private float _timeToDecrease;
    [SerializeField] private float _decreaseRatePerSecond;
    private bool _decreasing = false;
    private float _decreaseTimer;

    public void Awake()
    {
        _excaliburBar = transform.Find("ExcaliburBar").GetComponent<Image>();
    }

    public void Update()
    {

        _decreaseTimer += Time.deltaTime;

        //Check if charge needs to decrease
        if (_decreaseTimer > _timeToDecrease)
        {
            _decreasing = true;
        }
        else
        {
            _decreasing = false;
        }
        //Charge is decreasing - lower charge and make any 
        if (_decreasing)
        {
            Charge -= Time.deltaTime * _decreaseRatePerSecond;
        }

        SetBarFill(_charge);
    }

    /// <summary>
    /// Sets charge to zero
    /// </summary>
    public void ResetCharge()
    {
        Charge = 0;
    }

    /// <summary>
    /// Adds charge to Excalibur and resets time until the charge decreases
    /// </summary>
    public void IncreaseCharge(float amount)
    {
        _decreaseTimer = 0;
        Charge += amount;
    }

    public void ActivateAbility(int abilityNum)
    {
        if (Mathf.Approximately(Charge, 100.0f))
        {
            ResetCharge();

            switch (abilityNum)
            {
                case 1:
                    _abilityOne.Activate();
                    break;
                case 2:
                    _abilityTwo.Activate();
                    break;
            }
        }
    }


    // ***** UI METHODS *****
    #region UI

    /// <summary>
    /// Sets the UI image fill amount
    /// </summary>
    public void SetBarFill(float amount)
    {
        _excaliburBar.fillAmount = (amount / 100);
    }


    #endregion

    // ***** INPUT EVENTS ***** //
    #region InputEvents
    public void ExcaliburAbilityOneActivate(InputAction.CallbackContext context)
    {
        ActivateAbility(1);
    }

    public void ExcaliburAbilityTwoActivate(InputAction.CallbackContext context)
    {
        ActivateAbility(2);
    }
    
    #endregion
}
