using UnityEngine;

public class WitchboltObject : DamageObject
{
    
    [SerializeField] protected AudioClip hitSound;
    [SerializeField] protected ParticleSystem explosionParticles;
    [SerializeField] protected PushEnemy pushEnemy;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log("PushEnemy called");
            pushEnemy.Push(other);
            Explode();
        }
        // Will explode on collision objects in the environment, enemies, and destructible objects. Will not explode on collision with the player.
        if (other.gameObject.CompareTag("EnvironmentObject") || other.gameObject.CompareTag("Destructible"))
        {
            Explode();
        }
    }

    /// <summary>
    /// Explosion - play hit sound, create particles, destroy.
    /// The particles and sound clip are responsible for deleting themselves
    /// </summary>
    private void Explode()
    {
        AudioSource.PlayClipAtPoint(hitSound, transform.position);
        Instantiate(explosionParticles, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
