using UnityEngine;

public class TidalwaveSpell : Spell
{
    /// <summary>
    /// Create witchbolt and launch it using the projectile component
    /// </summary>
    /// <param name="player"Reference to the player</param>
    public override void Cast(Player player)
    {
        base.Cast(player);

        // TODO: We will want to eventually add code to align rotation of sprite with movement direction

        GameObject twObj = Instantiate(spellObj, player.transform.position, player.transform.rotation);
        Projectile fbProjectile = twObj.GetComponent<Projectile>();
        fbProjectile.LaunchProjectile(player.AimDirNorm);
    }
}
