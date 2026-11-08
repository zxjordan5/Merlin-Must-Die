using UnityEngine;

public class WitchboltObject : SpellObject
{
    [SerializeField] protected AudioClip hitSound;
    [SerializeField] protected ParticleSystem explosionParticles;
    [SerializeField] protected PushEnemy pushEnemy;
    

    protected override void EnemyCollision(Collider2D other)
    {
        base.EnemyCollision(other);

        pushEnemy.Push(other);
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
    /// </summary>
    protected void Explode()
    {
        AudioSource.PlayClipAtPoint(hitSound, transform.position);
        Instantiate(explosionParticles, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
