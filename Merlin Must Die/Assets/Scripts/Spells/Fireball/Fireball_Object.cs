using Unity.VisualScripting;
using System.Collections;
using UnityEngine;

/// <summary>
/// Fireball projectile that on collision will explode
/// Damage not currently implemented
/// </summary>
public class Fireball_Object : MonoBehaviour
{

    [SerializeField] float damage = 1;

    [SerializeField] protected AudioClip hitSound;
    [SerializeField] protected ParticleSystem explosionParticles;


    /// <summary>
    /// Collision logic for the fireball
    /// 
    /// TODO:
    /// change this to work with enemies.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Dummy")
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
