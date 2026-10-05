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
        GameObject fb_Obj = Instantiate(spellObj, player.transform.position, player.transform.rotation);
        Projectile fb_Projectile = fb_Obj.GetComponent<Projectile>();
        fb_Projectile.LaunchProjectile(player.AimDirNorm);
    }
}
