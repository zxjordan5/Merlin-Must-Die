using UnityEngine;

/// <summary>
/// Holds data for the Lightning Bolt spell. Inherits from Spell.cs
/// </summary>
public class LightningBoltSpell : Spell
{ 
    public override void Cast(Player player)
    {
        base.Cast(player);
        Quaternion objRot = Quaternion.Euler(0, 0, player.AimAngle);
        GameObject fbObj = Instantiate(spellObj, player.transform.position, objRot);
    }
}
