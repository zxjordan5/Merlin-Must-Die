using UnityEngine;

/// <summary>
/// Produces a shake effect on hit. SHOULD ONLY BE USED ON STATIC OBJECTS
/// </summary>
public class ShakeOnHit : MonoBehaviour
{
    /// <summary>
    /// Variable that determines amount the object shakes when hit
    /// </summary>
    [SerializeField] private float shakeAmount = 0.1f;
    
    /// <summary>
    /// State check for the object and whether or not it is currenty doing a shake effect
    /// </summary>
    private bool _isShaking;

    /// <summary>
    /// Starting position of the object
    /// </summary>
    private Vector2 _startPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startPosition = transform.position;
        _isShaking = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Shaking effect when hit
        if (_isShaking)
        {
            transform.position = _startPosition + UnityEngine.Random.insideUnitCircle * shakeAmount;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<DamageObject>())
        {
            // Object isn't destroyed from the damage, make it shake and flash white
            _startPosition = transform.position; //Update current position before changing it through shaking
            _isShaking = true;
            Invoke(nameof(ResetShake), shakeAmount);
        }
    }
    
    void ResetShake()
    {
        _isShaking = false;
        transform.position = _startPosition;
    }
}
