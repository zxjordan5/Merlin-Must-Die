using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for any type of menu action such as game starting
/// </summary>

public abstract class MenuAction : ScriptableObject
{
    
    public abstract void Action();
}
