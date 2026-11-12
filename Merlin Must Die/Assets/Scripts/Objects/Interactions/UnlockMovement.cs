using System;
using UnityEngine;

[System.Serializable]
public class UnlockMovement : Interaction
{
    public override void Behaviour()
    {
        FindAnyObjectByType<Player>().CanMove = true;
    }
}
