using System;
using UnityEngine;

public class Fireball_Spell : Spell
{
    [SerializeField] float initial_velocity;
    
    //Create fireball and launch in the direction the player is facing
    public override void Cast(Player player)
    {
        GameObject fb_Obj = Instantiate(spellObj);
        Projectile fb_Projectile = fb_Obj.GetComponent<Projectile>();
        fb_Projectile.Velocity = new Vector2(initial_velocity, initial_velocity) * player.AimDirNorm;
    }
}
