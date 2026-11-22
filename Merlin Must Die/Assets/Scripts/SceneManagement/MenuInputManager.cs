using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuInputManager : MonoBehaviour
{

    //Current Input
    private List<Globals.Direction> _playerInputSequence = new List<Globals.Direction>();


    //Assigned Menu Action Fields
    [SerializeField]
    public List<MenuOption> menuOptions;
    private List<MenuAction> _menuActions = new List<MenuAction>();

    private MenuAction _preparedAction;

    private void Awake()
    {
        foreach(MenuOption option in menuOptions)
        {
            _menuActions.Add(option.Action);
        }
    }

    /// <summary>
    /// Adds the current input to the sequence, checks if any actions match
    /// </summary>
    /// <param name="input">The direction of the new input</param>
    private void ProcessInput(Globals.Direction input)
    {
        //Menu inputs should be no longer than 2 inputs
        if (_playerInputSequence.Count > 2)
        {
            ResetInputSequence();
        }

        //Will only read input if a spell is not prepared
        if (!_preparedAction)
        {
            // Makes the input icons appear above the player's head when a spell is not prepared
            _playerInputSequence.Add(input);
        }

        CheckForMatches();
    }

    private void ResetInputSequence()
    {
        _playerInputSequence.Clear();

        //Reset input icon UI
        foreach (MenuOption option in menuOptions)
        {
            option.ResetArrows();
        }
    }

    /// <summary>
    /// Checks every action against the current input sequence to see if there is a match
    /// If so, sets the current prepared action to the matched action
    /// </summary>
    private void CheckForMatches()
    {
        int mismatches = 0; //Track incorrect sequences

        //Loop every option
        foreach (MenuOption option in menuOptions)
        {
            MenuAction action = option.Action;
            int dirMatches = 0;

            //Loop through the players input, comparing it with the current action
            for (int i = 0; i < _playerInputSequence.Count; i++)
            {
                if (_playerInputSequence[i] == option.InputSequence[i])
                {
                    dirMatches++;
                    option.ArrowPressed(i);
                }
                else
                {
                    mismatches++;
                    break;
                }
            }
            //All of the current actions match
            if (dirMatches == option.InputSequence.Count)
            {
                _preparedAction = action;
            }
        }
        //No input matches, reset
        if (mismatches >= _menuActions.Count)
        {
            ResetInputSequence();
        }
    }

    private void ActivateAction()
    {
        if (_preparedAction)
        {
            _preparedAction.Action();
        }
        _preparedAction = null;
        ResetInputSequence();
    }

    // ***** INPUT METHODS *****
    #region Input Methods
    public void OnPrepareSpellUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ProcessInput(Globals.Direction.Up);
        }
    }

    public void OnPrepareSpellDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ProcessInput(Globals.Direction.Down);
        }
    }

    public void OnPrepareSpellLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ProcessInput(Globals.Direction.Left);
        }
    }

    public void OnPrepareSpellRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ProcessInput(Globals.Direction.Right);
        }
    }

    public void OnCast(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ActivateAction();
        }
    }
    #endregion
}
