using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Assigns the input map through a hardcoded script.
/// Add the event to ConnectInputMap and DisconnctInputMap with the function
/// 
/// Checks for the current active input scheme
/// </summary>
public class InputManager : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }
    public enum ControlScheme { Unknown = 0, Keyboard = 1, Controller = 2 }

    private Player _player;
    private Grimoire _grimoire;
    private Excalibur _excalibur;

    private void Awake()
    {
        if(Instance == null)
            Instance = GetComponent<PlayerInput>();
    }

    /// <summary>
    /// Connects the events when enabled
    /// </summary>
    private void OnEnable()
    {
        _player = GameManager.Player;
        _grimoire = GameManager.Grimoire;
        _excalibur = GameManager.Excalibur;

        ConnectInputMap();
    }

    /// <summary>
    /// Disconnect events when disabled
    /// </summary>
    private void OnDisable()
    {
        DisconnectInputMap();
    }

    /// <summary>
    /// Hard coded input connections!
    /// </summary>
    public void ConnectInputMap()
    {
        if (_player)
        {
            ConnectIndividualAction("KBM_AutoMove", _player.OnKBM_AutoMove);
            ConnectIndividualAction("KBM_Aim", _player.OnKBM_Aim);
            ConnectIndividualAction("GP_Move", _player.OnGP_Move);
            ConnectIndividualAction("GP_Aim", _player.OnGP_Aim);
            ConnectIndividualAction("Interact", _player.Interact);
        }
        if (_grimoire)
        {
            ConnectIndividualAction("PrepareSpellUp", _grimoire.OnPrepareSpellUp);
            ConnectIndividualAction("PrepareSpellDown", _grimoire.OnPrepareSpellDown);
            ConnectIndividualAction("PrepareSpellLeft", _grimoire.OnPrepareSpellLeft);
            ConnectIndividualAction("PrepareSpellRight", _grimoire.OnPrepareSpellRight);
            ConnectIndividualAction("CastSpell", _grimoire.OnCast);
        }
        if (_excalibur) 
        { 
            ConnectIndividualAction("ExcaliburA", _excalibur.ExcaliburAbilityOneActivate);
            ConnectIndividualAction("ExcaliburB", _excalibur.ExcaliburAbilityTwoActivate);
        }
    }

    public void DisconnectInputMap()
    {
        if (_player) 
        { 
            DisconnectIndividualAction("KBM_AutoMove", _player.OnKBM_AutoMove);
            DisconnectIndividualAction("KBM_Aim", _player.OnKBM_Aim);
            DisconnectIndividualAction("GP_Move", _player.OnGP_Move);
            DisconnectIndividualAction("GP_Aim", _player.OnGP_Aim);
            DisconnectIndividualAction("Interact", _player.Interact);
        }
        if (_grimoire)
        {
            DisconnectIndividualAction("PrepareSpellUp", _grimoire.OnPrepareSpellUp);
            DisconnectIndividualAction("PrepareSpellDown", _grimoire.OnPrepareSpellDown);
            DisconnectIndividualAction("PrepareSpellLeft", _grimoire.OnPrepareSpellLeft);
            DisconnectIndividualAction("PrepareSpellRight", _grimoire.OnPrepareSpellRight);
            DisconnectIndividualAction("CastSpell", _grimoire.OnCast);
        }
        if(_excalibur)
        { 
            DisconnectIndividualAction("ExcaliburA", _excalibur.ExcaliburAbilityOneActivate);
            DisconnectIndividualAction("ExcaliburB", _excalibur.ExcaliburAbilityTwoActivate);
        }
    }

    public void ConnectIndividualAction(string actionName, System.Action<InputAction.CallbackContext> action)
    {
        Instance.actions[actionName].started += action;
        Instance.actions[actionName].performed += action;
        Instance.actions[actionName].canceled += action;
    }
    public void DisconnectIndividualAction(string actionName, System.Action<InputAction.CallbackContext> action)
    {
        Instance.actions[actionName].started -= action;
        Instance.actions[actionName].performed -= action;
        Instance.actions[actionName].canceled -= action;
    }


    /// <summary>
    /// Loops through all active devices being read in playerInput to find the current device being used
    /// If detecting a controller, will prioritize the controller
    /// </summary>
    public static ControlScheme CurrentDevice
    {
        get
        {
            bool controllerFound = false;
            bool keyboardFound = false;
            foreach (InputDevice device in InputSystem.devices)
            {
                if (device is Gamepad)
                {
                    controllerFound = true;
                }
                else if (device is Keyboard)
                {
                    keyboardFound = true;
                }
            }
            if (controllerFound)
                return ControlScheme.Controller;
            else if(keyboardFound)
                return ControlScheme.Keyboard;
            else
                return ControlScheme.Unknown;
        }
    }
}