using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Earth Barrier will lift earth out of the ground and create a temporary barrier
/// </summary>
[Serializable]
public class EarthBarrierSpell : Spell
{
    /// <summary>
    /// Calls dash method on player
    /// Creates particles that are attached to the player but function in world space
    /// </summary>
    public override void Cast(Player player)
    {
        base.Cast(player);

        GameObject earthBarrier = Instantiate(spellObj, player.transform.position, player.transform.rotation);
        Projectile earthProj = earthBarrier.GetComponent<Projectile>();
        earthProj.LaunchProjectile(player.AimDirNorm););

        if (player.Velocity > 1)
        {
            earthProj.UsePlayerVelocity(player.Velocity);
        }
    }
}
