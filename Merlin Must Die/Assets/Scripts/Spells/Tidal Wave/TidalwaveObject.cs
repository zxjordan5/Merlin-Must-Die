using UnityEngine;

public class TidalwaveObject : SpellObject
{
    [SerializeField] protected AudioClip hitSound;
    [SerializeField] protected ParticleSystem splashParticles;

    [SerializeField] protected float _pushDelay;
    [SerializeField] protected PushEnemy pushEnemy;

    protected override void EnemyCollision(Collider2D other)
    {
        base.EnemyCollision(other);

        pushEnemy.Push(other);
    }

    protected override void ObjectCollision(Collider2D other)
    {
        base.ObjectCollision(other);

        Splash();
    }


    /// <summary>
    /// Splash - play hit sound, create particles, destroy.
    /// The particles and sound clip are responsible for deleting themselves
    /// </summary>
    private void Splash()
    {
        AudioSource.PlayClipAtPoint(hitSound, transform.position);
        Instantiate(splashParticles, transform.position, transform.rotation);
        Destroy(gameObject);
    }

}
