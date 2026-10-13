using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Script for destructible objects within the game
/// </summary>
public class DestructibleObject : MonoBehaviour
{
    /// <summary>
    /// Health of the object (varies depending on object)
    /// </summary>
    [SerializeField] protected int health;
    

    /// <summary>
    /// Reference to the prefab of the destroyed object
    /// </summary>
    [SerializeField] protected UnityEngine.Object destructiblePrefabRef;

    /// <summary>
    /// Reference to the particle effect that will play when the object is destroyed
    /// </summary>
    [SerializeField] protected UnityEngine.Object particleExplosionRef;

    /// <summary>
    /// Sprite to use when object is full or mostly full health
    /// </summary>
    [SerializeField] private Sprite fullHealthSprite;

    // Sprite to use when object passes a certain damage threshold
    [SerializeField] private Sprite damagedSprite;

    /// <summary>
    /// Variable that determines amount the object shakes when hit
    /// </summary>
    [SerializeField] private float shakeAmount = 0.1f;

    /// <summary>
    /// How long the barrel flashes for after getting hit
    /// </summary>
    [SerializeField] private float flashDuration = 0.2f;

    /// <summary>
    /// SpriteRenderer for the object. We need to reference this to change sprites
    /// </summary>
    private SpriteRenderer _spriteRenderer;
    
    /// <summary>
    /// Initial health of the object
    /// </summary>
    private int _initialHealth;

    /// <summary>
    /// Original color of the object
    /// </summary>
    private Color _originalColor;
    
    /// <summary>
    /// State check for the object and whether or not it is currenty doing a shake effect
    /// </summary>
    private bool _isShaking;

    /// <summary>
    /// Starting position of the object
    /// </summary>
    private Vector2 _startPosition;
    
    /// <summary>
    /// Coroutine for the flash effect when the object is hit.
    /// Reference is needed to check for null and to see if the coroutine needs to be reset
    /// </summary>
    private Coroutine _flashCoroutine;

    void Awake()
    {
        _startPosition = transform.position;
        _initialHealth = health;
        _isShaking = false;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor =  _spriteRenderer.color;
    }

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
        if(other.CompareTag("DamageSpell"))
        {
            DamageObject dmgObj = other.GetComponent<DamageObject>();
            TakeDamage(dmgObj.Damage);

            if (_spriteRenderer.sprite != damagedSprite && health <= _initialHealth / 2)
            {
                SwitchToDamagedSprite();
            }
            
            if (health <= 0)
            {
                DestroyObject();
            }
            else
            {
                // Object isn't destroyed from the damage, make it shake and flash white
                _isShaking = true;
                Invoke(nameof(ResetShake), shakeAmount);
                Invoke(nameof(TriggerFlash), 0.0f);
            }
        }
    }

    /// <summary>
    /// Subtracts a given damage amount from the current health of the object
    /// </summary>
    /// <param name="damage">The damage being taken</param>
    void TakeDamage(int damage)
    {
        health -= damage;
    }

    /// <summary>
    /// Deletes this object from the scene and instantiates it's destroyed pieces
    /// </summary>
    void DestroyObject()
    {
        GameObject destroyedObject = (GameObject)Instantiate(destructiblePrefabRef, transform.position, transform.rotation);
        GameObject particles = (GameObject)Instantiate(particleExplosionRef, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    /// <summary>
    /// Switches the sprite of the object to its "damaged" variant
    /// </summary>
    void SwitchToDamagedSprite()
    {
        _spriteRenderer.sprite = damagedSprite;
    }

    void ResetShake()
    {
        _isShaking = false;
        transform.position = _startPosition;
    }

    void TriggerFlash()
    {
        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
        }

        _flashCoroutine = StartCoroutine(FlashOnHit());
    }

    /// <summary>
    /// Master coroutine for the flashing sequence. Calls the two coroutines that create and reset the flashing effect on an object
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlashOnHit()
    {
        yield return Flash();
        yield return ResetFlash();
    }

    /// <summary>
    /// Turns the sprite of the hit object white
    /// </summary>
    /// <returns></returns>
    private IEnumerator Flash()
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < flashDuration / 2.0f)
        {
            elapsedTime += Time.deltaTime;
            // LERPs from original color to white
            float newColorR = Mathf.Lerp(_originalColor.r, 255.0f, elapsedTime / (flashDuration / 2));
            float newColorG = Mathf.Lerp(_originalColor.g, 255.0f, elapsedTime / (flashDuration / 2));
            float newColorB = Mathf.Lerp(_originalColor.b, 255.0f, elapsedTime / (flashDuration / 2));

            _spriteRenderer.color = new Color(newColorR, newColorG, newColorB,  _originalColor.a);
            
            // All this does is wait until the next frame call
            // The reason this is an IEnumerator is so that we don't have to stick update where it's not needed
            yield return null;
        }
    }

    /// <summary>
    /// Fades the sprite of the hit object back to normal after flashing
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetFlash()
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < flashDuration / 2.0f)
        {
            elapsedTime += Time.deltaTime;
            // LERPs from current color back to the original
            float newColorR = Mathf.Lerp(_spriteRenderer.color.r, _originalColor.r, elapsedTime / (flashDuration / 2));
            float newColorG = Mathf.Lerp(_spriteRenderer.color.g, _originalColor.g, elapsedTime / (flashDuration / 2));
            float newColorB = Mathf.Lerp(_spriteRenderer.color.b, _originalColor.b, elapsedTime / (flashDuration / 2));

            _spriteRenderer.color = new Color(newColorR, newColorG, newColorB,  _originalColor.a);
            
            // All this does is wait until the next frame call
            // The reason this is an IEnumerator is so that we don't have to stick update where it's not needed
            yield return null;
        }
    }
}