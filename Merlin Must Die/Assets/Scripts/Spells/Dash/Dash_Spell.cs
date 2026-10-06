using System;
using System.Collections;
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
    /// Creates particles that are attached to the player but function in world space
    /// </summary>
    public override void Cast(Player player)
    {
        base.Cast(player);

        GameObject dashObj = Instantiate(spellObj, player.transform);
        ParticleSystem dashParticles = dashObj.GetComponentInChildren<ParticleSystem>();
        var main = dashParticles.main;
        main.startRotation = (float)Math.Atan2(player.AimDirNorm.x, player.AimDirNorm.y);
        StartCoroutine(StartParticles(dashObj, dashParticles));

        player.Dash(dashSpeed, dashTime);
    }

    /// <summary>
    /// Timer to stop particles after dash is over and delete them some time after
    /// </summary>
    private IEnumerator StartParticles(GameObject dashObj, ParticleSystem dashParticles)
    {
        yield return new WaitForSeconds(dashTime * .8f);
        dashParticles.Stop();
        Destroy(dashObj, dashTime * 2);
    }
}
