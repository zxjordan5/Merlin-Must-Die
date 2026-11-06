using UnityEngine;

public class PushEnemy : MonoBehaviour
{
    [SerializeField] protected float pushForce = 5.0f;

    /// <summary>
    /// Called when an enemy is hit. Pushes the enemy back in the opposite direction of the hit
    /// </summary>
    /// <param name="other"></param>
    public void Push(Collider2D other)
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
}
