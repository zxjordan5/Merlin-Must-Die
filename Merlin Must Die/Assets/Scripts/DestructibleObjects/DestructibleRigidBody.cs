using System.Collections;
using Microsoft.Extensions.Logging.Abstractions;
using TMPro;
using UnityEngine;

/// <summary>
/// Configures torque and force for all destructable object components. Attach to any destructable object "part"
/// </summary>
public class DestructibleRigidBody : MonoBehaviour
{
    /// <summary>
    /// The average desired forceDirection for the destructable object
    /// </summary>
    [SerializeField] private Vector2 forceDirection;

    /// <summary>
    /// Offset for random force applied in X direction. Range for force in the X direction will be [forceDirection - offset, forceDirection + offset]
    /// </summary>
    [SerializeField] private float forceDirectionOffsetX = 25.0f;
    
    /// <summary>
    /// Offset for random force applied in Y direction. Range for force in the Y direction will be [forceDirection, forceDirection + offset]
    /// </summary>
    [SerializeField] private float forceDirectionOffsetY = 25.0f;
    
    /// <summary>
    /// Max torque applied to the destructable object when it's destroyed. The torque value will be between [-torque, torque]
    /// </summary>
    [SerializeField] private float torque = 50.0f;

    /// <summary>
    /// Timer for how long a destroyed piece remains on screen before getting destroyed
    /// </summary>
    [SerializeField] private float destructionTimer = 1.0f;

    /// <summary>
    /// Timer for how long a it takes for a piece to visually "fade out" before being removed from the scene
    /// </summary>
    [SerializeField] private float fadeOutTimer = 0.5f;

    /// <summary>
    /// Collider attached to the rigidbody
    /// </summary>
    private Collider2D _collider;
    
    /// <summary>
    /// Sprite renderer attached to the component
    /// </summary>
    private SpriteRenderer _spriteRenderer;
    

    private Rigidbody2D _rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        float randTorque = UnityEngine.Random.Range(-torque, torque);
        float randForceX = UnityEngine.Random.Range(forceDirection.x - forceDirectionOffsetX, forceDirection.x + forceDirectionOffsetX);
        float randForceY = UnityEngine.Random.Range(forceDirection.y, forceDirection.y + forceDirectionOffsetY);
        
        _rb = GetComponent<Rigidbody2D>();
        _rb.AddForce(new Vector2(randForceX, randForceY));
        _rb.AddTorque(randTorque);

        Invoke(nameof(TriggerFadeOut), destructionTimer);
    }

    /// <summary>
    /// Triggers fade out visual effect
    /// </summary>
    private void TriggerFadeOut()
    {
        StartCoroutine(FadeAndDestroySelf());
    }

    /// <summary>
    /// Fades out the destroyed piece and deletes it from the current scene
    /// </summary>
    private IEnumerator FadeAndDestroySelf()
    {
        // Disable collision
        _collider.enabled = false;
        
        // Fade out visual effect
        Color originalColor = _spriteRenderer.color;
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeOutTimer)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(originalColor.a, 0.0f, elapsedTime / fadeOutTimer);
            _spriteRenderer.color = new Color(originalColor.r,  originalColor.g, originalColor.b, newAlpha);
            
            // All this does is wait until the next frame call
            // The reason this is an IEnumerator is so that we don't have to stick update where it's not needed
            yield return null;
        }
        
        // Delete from scene
        Destroy(gameObject);
    }
}
