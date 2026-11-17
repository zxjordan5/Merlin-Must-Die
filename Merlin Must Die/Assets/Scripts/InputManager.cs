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
    private static PlayerInput _playerInput;
    public enum ControlScheme { Unknown = 0, Keyboard = 1, Controller = 2 }

    [SerializeField] private Player _player;
    [SerializeField] private Grimoire _grimoire;
    [SerializeField] private Excalibur _excalibur;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        if(!_player)
            _player = FindAnyObjectByType<Player>();
        if(!_grimoire)
            _grimoire = FindAnyObjectByType<Grimoire>();
        if (!_excalibur)
            _excalibur = FindAnyObjectByType<Excalibur>();
    }

    /// <summary>
    /// Connects the events when enabled
    /// </summary>
    private void OnEnable()
    {
        ConnectInputMap();
    }

    /// <summary>
    /// Apparently Unity doesn't unassign the input connections so without disconnecting the inputs
    /// there will be memory leaks. Without this it causes multiple input assignments after closing the In-Editor player
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
        ConnectIndividualAction("KBM_AutoMove", _player.OnKBM_AutoMove);
        ConnectIndividualAction("KBM_Aim", _player.OnKBM_Aim);
        ConnectIndividualAction("GP_Move", _player.OnGP_Move);
        ConnectIndividualAction("GP_Aim", _player.OnGP_Aim);
        ConnectIndividualAction("PrepareSpellUp", _grimoire.OnPrepareSpellUp);
        ConnectIndividualAction("PrepareSpellDown", _grimoire.OnPrepareSpellDown);
        ConnectIndividualAction("PrepareSpellLeft", _grimoire.OnPrepareSpellLeft);
        ConnectIndividualAction("PrepareSpellRight", _grimoire.OnPrepareSpellRight);
        ConnectIndividualAction("CastSpell", _grimoire.OnCast);
        ConnectIndividualAction("ExcaliburA", _excalibur.ExcaliburAbilityOneActivate);
        ConnectIndividualAction("ExcaliburB", _excalibur.ExcaliburAbilityTwoActivate);
        ConnectIndividualAction("Interact", _player.Interact);
    }

    public void DisconnectInputMap()
    {
        DisconnectIndividualAction("KBM_AutoMove", _player.OnKBM_AutoMove);
        DisconnectIndividualAction("KBM_Aim", _player.OnKBM_Aim);
        DisconnectIndividualAction("GP_Move", _player.OnGP_Move);
        DisconnectIndividualAction("GP_Aim", _player.OnGP_Aim);
        DisconnectIndividualAction("PrepareSpellUp", _grimoire.OnPrepareSpellUp);
        DisconnectIndividualAction("PrepareSpellDown", _grimoire.OnPrepareSpellDown);
        DisconnectIndividualAction("PrepareSpellLeft", _grimoire.OnPrepareSpellLeft);
        DisconnectIndividualAction("PrepareSpellRight", _grimoire.OnPrepareSpellRight);
        DisconnectIndividualAction("CastSpell", _grimoire.OnCast);
        DisconnectIndividualAction("ExcaliburA", _excalibur.ExcaliburAbilityOneActivate);
        DisconnectIndividualAction("ExcaliburB", _excalibur.ExcaliburAbilityTwoActivate);
        DisconnectIndividualAction("Interact", _player.Interact);
    }

    public void ConnectIndividualAction(string actionName, System.Action<InputAction.CallbackContext> action)
    {
        _playerInput.actions[actionName].started += action;
        _playerInput.actions[actionName].performed += action;
        _playerInput.actions[actionName].canceled += action;
    }
    public void DisconnectIndividualAction(string actionName, System.Action<InputAction.CallbackContext> action)
    {
        _playerInput.actions[actionName].started -= action;
        _playerInput.actions[actionName].performed -= action;
        _playerInput.actions[actionName].canceled -= action;
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
            foreach (InputDevice device in _playerInput.devices)
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