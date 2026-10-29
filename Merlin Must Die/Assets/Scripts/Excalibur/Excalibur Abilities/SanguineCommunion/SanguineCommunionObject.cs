using UnityEngine;

public class SanguineCommunionObject : DamageObject
{
    [SerializeField] protected AudioClip hitSound;
    [SerializeField] protected ParticleSystem explosionParticles;

    /// <summary>
    /// Reference to the player in the scene
    /// </summary>
    private Player _player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = FindAnyObjectByType<Player>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
           _player.Heal();
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
    /// Damage logic may eventually go here
    /// </summary>
    private void Explode()
    {
        if(hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }
        if(explosionParticles != null)
        {
            Instantiate(explosionParticles, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}
