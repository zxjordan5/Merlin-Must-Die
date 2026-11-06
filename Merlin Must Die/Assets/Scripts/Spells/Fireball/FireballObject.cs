using UnityEngine;

/// <summary>
/// Fireball projectile that on collision with an enemy or destructible object will explode
/// </summary>
public class FireballObject : SpellObject
{
    [SerializeField] protected AudioClip hitSound;
    [SerializeField] protected ParticleSystem explosionParticles;
    
    protected override void EnemyCollision(Collider2D other)
    {
        base.EnemyCollision(other);

        Explode();
    }

    protected override void ObjectCollision(Collider2D other)
    {
        base.ObjectCollision(other);

        Explode();
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
