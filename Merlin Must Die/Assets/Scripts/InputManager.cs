using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static PlayerInput _playerInput;
    public enum ControlScheme { Unknown = 0, Keyboard = 1, Controller = 2 }

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
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