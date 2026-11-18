using System;
using UnityEngine;

/// <summary>
/// Fireball spell for testing
/// </summary>
[Serializable]
public class FireballSpell : Spell
{
    /// <summary>
    /// Create fireball and launch it using the projectile component
    /// </summary>
    /// <param name="player"></param>
    public override void Cast(Player player)
    {
        base.Cast(player);

        if (castSound == null)
        {
            GameObject audio = GameObject.Find("Audio/Fireball");
            castSound = audio.GetComponent<AudioSource>();
        }
        castSound.Play();

        GameObject fbObj = Instantiate(spellObj, player.transform.position, player.transform.rotation);
        Projectile fbProjectile = fbObj.GetComponent<Projectile>();
        fbProjectile.LaunchProjectile(player.AimDirNorm);
    }
}
