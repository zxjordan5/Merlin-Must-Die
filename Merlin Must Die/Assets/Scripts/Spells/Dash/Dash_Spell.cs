using System;
using UnityEngine;

/// <summary>
/// Dash spell for testing
/// </summary>
[Serializable]
public class Dash_Spell : Spell
{
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;

    /// <summary>
    /// Calls dash method on player
    /// </summary>
    /// <param name="player"></param>
    public override void Cast(Player player)
    {
        base.Cast(player);

        GameObject dash_Obj = Instantiate(spellObj, player.transform.position, player.transform.rotation);
        player.Dash(dashSpeed, dashTime);
    }
}
