using UnityEngine;

public class TidalwaveObject : MonoBehaviour
{
    [SerializeField] protected AudioClip hitSound;
    [SerializeField] protected ParticleSystem splashParticles;

    [SerializeField] protected float _pushDelay;
    [SerializeField] protected PushEnemy _pushEnemy;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log("PushEnemy called");
            _pushEnemy.Push(other);
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
        Debug.Log("destroying");
        AudioSource.PlayClipAtPoint(hitSound, transform.position);
        Instantiate(splashParticles, transform.position, transform.rotation);
        Destroy(gameObject);
    }

}
