using System;
using UnityEngine;

/// <summary>
/// Fireball spell for testing
/// </summary>
public class Fireball_Spell : Spell
{
    //Create fireball and launch in the direction the player is facing
    public override void Cast(Player player)
    {
        //Default cooldown check
        /* Ideally there would be a way to have this check be part of the base spell class
        and call the base implementation. Currently don't know how to have the method cancel
        if calling the base cast function. Can also include cast sound in the base class.
        */
        if (OnCooldown)
        {
            return;
        }
        ResetCooldown();

        AudioSource.PlayClipAtPoint(castSound, player.transform.position); //Play cast sound

        //Create the fireball and launch it using the projectile component
        GameObject fb_Obj = Instantiate(spellObj, player.transform);
        Projectile fb_Projectile = fb_Obj.GetComponent<Projectile>();
        fb_Projectile.LaunchProjectile(player.AimDirNorm);
    }
}
