using System;
using UnityEngine;

/// <summary>
/// Fireball spell for testing
/// </summary>
[Serializable]
public class Fireball_Spell : Spell
{
    /// <summary>
    /// Create fireball and launch it using the projectile component
    /// </summary>
    /// <param name="player"></param>
    public override void Cast(Player player)
    {
        base.Cast(player);

        GameObject fbObj = Instantiate(spellObj, player.transform.position, player.transform.rotation);
        Projectile fbProjectile = fbObj.GetComponent<Projectile>();
        fbProjectile.LaunchProjectile(player.AimDirNorm);
    }
}
