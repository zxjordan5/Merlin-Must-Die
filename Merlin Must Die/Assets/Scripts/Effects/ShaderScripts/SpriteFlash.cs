using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Produces an effect where a sprite will flash over a certain amount of time
/// </summary>
public class SpriteFlash : MonoBehaviour
{
    /// <summary>
    /// List of tags to check on collision to see if the flash animation should play.
    /// EX) Enemy should have the tag DamageSpell in this list
    /// </summary>
    [SerializeField] private List<string> tagsToCheck;
    
    /// <summary>
    /// How fast the flash effect fades away
    /// </summary>
    [SerializeField] private float flashDecaySpeed = 1.0f;

    /// <summary>
    /// How many times the flash effect should play
    /// </summary>
    [SerializeField] private int timesToFlash = 1;
    
    
    /// <summary>
    /// All sprite renderers on the object
    /// </summary>
    private SpriteRenderer[] _spriteRenderers;
    
    /// <summary>
    /// Property block of the material of the sprite
    /// </summary>
    private MaterialPropertyBlock _propertyBlock;
    
    /// <summary>
    /// Value used to determine what color the sprite should be during the flash. Values are 0.0-1.0 (normal to fully white sprite)
    /// </summary>
    private float _blinkFactor;

    private int _flashesRemaining;

    private void Start()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        _propertyBlock = new MaterialPropertyBlock();
        Debug.Log(gameObject.name + " " + _spriteRenderers);
    }

    private void Update()
    {
        // Reset to flash again
        if (_blinkFactor <= 0.0f)
        {
            return;
        }
        _blinkFactor = Mathf.Lerp(_blinkFactor, 0.0f, Time.deltaTime * flashDecaySpeed);
        
        // Epsilon value to prevent floating point errors
        if (_blinkFactor <= 0.01f)
        {
            _blinkFactor = 0.0f;
            if (_flashesRemaining > 0)
            {
                _flashesRemaining--;
                _blinkFactor = 1.0f;
            }
        }
        ApplyBlinkFactor();
    }

    public void Flash()
    {
        if (timesToFlash <= 0)
        {
            return;
        }

        _flashesRemaining = timesToFlash - 1;
        _blinkFactor = 1.0f;
        ApplyBlinkFactor();
    }

    private void ApplyBlinkFactor()
    {
        foreach (SpriteRenderer renderer in _spriteRenderers)
        {
            renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat("_BlinkFactor", _blinkFactor);
            renderer.SetPropertyBlock(_propertyBlock);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        int tagsFound = 0;
        foreach (string tag in tagsToCheck)
        {
            if (!other.CompareTag(tag))
            {
                return;
            }
            tagsFound++;
        }

        // We didn't hit something that should play the flash
        if (tagsFound == 0) return;

        // We hit something that should play the flash
        Flash();
    }
}
