using Unity.VisualScripting;
using System.Collections;
using UnityEngine;

/// <summary>
/// Fireball projectile that on collision with an enemy or destructible object will explode
/// </summary>
public class Fireball_Object : DamageObject
{
    [SerializeField] protected AudioClip hitSound;
    [SerializeField] protected ParticleSystem explosionParticles;


    /// <summary>
    /// Collision logic for the fireball
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Will explode on collision objects in the environment, enemies, and destructible objects. Will not explode on collision with the player.
        if (other.CompareTag("EnvironmentObject") ||other.CompareTag("Enemy") || other.CompareTag("Destructible"))
        {
            Explode();
        }
    }

    /// <summary>
    /// Explosion - play hit sound, create particles, destroy.
    /// The particles and sound clip are responsible for deleting themselves
    /// Damage logic may eventually go here
    /// </summary>
    private void Explode()
    {
        AudioSource.PlayClipAtPoint(hitSound, transform.position);
        Instantiate(explosionParticles, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    
}
