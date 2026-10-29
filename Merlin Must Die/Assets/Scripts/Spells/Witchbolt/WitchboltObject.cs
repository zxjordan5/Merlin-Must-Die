using UnityEngine;

public class WitchboltObject : DamageObject
{
    [SerializeField] protected float pushForce = 5.0f;
    [SerializeField] protected AudioClip hitSound;
    [SerializeField] protected ParticleSystem explosionParticles;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("PushEnemy called");
            PushEnemy(other);
            Explode();
        }
        // Will explode on collision objects in the environment, enemies, and destructible objects. Will not explode on collision with the player.
        if (other.gameObject.CompareTag("EnvironmentObject") || other.gameObject.CompareTag("Destructible"))
        {
            Explode();
        }
    }

    /// <summary>
    /// Called when an enemy is hit. Pushes the enemy back in the opposite direction of the hit
    /// </summary>
    /// <param name="other"></param>
    private void PushEnemy(Collider2D other)
    {
        // Get the collider attached to this GameObject
        Collider2D collider = GetComponent<Collider2D>();
        RaycastHit2D[] hits = new RaycastHit2D[1];

        // Cast this collider towards the other collider to find the intersection details
        Vector2 direction = (other.transform.position - transform.position).normalized;
        int hitCount = collider.Cast(direction, hits, 10f);

        if (hitCount > 0)
        {
            Vector2 collisionNormal = hits[0].normal.normalized;
            Vector2 pushDirection = -1.0f * collisionNormal;
            Rigidbody2D otherRb = other.gameObject.GetComponent<Rigidbody2D>();

            if (otherRb != null)
            {
                otherRb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
            }
            else
            {
                Debug.Log("ERROR: Rigidbody of other could not be retrieved!");
            }
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
