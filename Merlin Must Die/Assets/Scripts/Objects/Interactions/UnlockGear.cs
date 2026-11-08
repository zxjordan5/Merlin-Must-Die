using System;
using UnityEngine;

[System.Serializable]
public class UnlockGear : Interaction
{
    public override void Behaviour()
    {
        FindAnyObjectByType<Player>().HasGear = true;
    }
}
