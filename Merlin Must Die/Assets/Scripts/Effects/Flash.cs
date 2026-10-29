using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{

    /// <summary>
    /// List of tags to check on collision to see if the flash animation should play.
    /// EX) Enemy should have the tag DamageSpell in this list
    /// </summary>
    [SerializeField] private List<string> tagsToCheck;

    /// <summary>
    /// How long the barrel flashes for after getting hit
    /// </summary>
    [SerializeField] private float flashDuration = 0.2f;

    /// <summary>
    /// How many times the flash animation should play
    /// </summary>
    [SerializeField] private int timesToFlash = 1;

    /// <summary>
    /// SpriteRenderer for the object. We need to reference this to change sprites
    /// </summary>
    private SpriteRenderer _spriteRenderer;

    /// <summary>
    /// Original color of the object
    /// </summary>
    private Color _originalColor;

    /// <summary>
    /// Coroutine for the flash effect when the object is hit.
    /// Reference is needed to check for null and to see if the coroutine needs to be reset
    /// </summary>
    private Coroutine _flashCoroutine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
    }

    void OnTriggerEnter2D(Collider2D other)
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
        Invoke(nameof(TriggerFlash), 0.0f);
    }

    /// <summary>
    /// Method that calls an individual flash. Can be referenced in another script if custom flash functionality is needed
    /// </summary>
    public void SingleFlash()
    {
        Invoke(nameof(TriggerFlash), 0.0f);
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
        int timesFlashed = 0;
        while (timesFlashed < timesToFlash)
        {
            yield return FlashEffect();
            yield return ResetFlash();
            timesFlashed++;
        }
    }

    /// <summary>
    /// Turns the sprite of the hit object white
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlashEffect()
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < flashDuration / 2.0f)
        {
            elapsedTime += Time.deltaTime;
            // LERPs from original color to white
            float newColorR = Mathf.Lerp(_originalColor.r, 255.0f, elapsedTime / (flashDuration / 2));
            float newColorG = Mathf.Lerp(_originalColor.g, 255.0f, elapsedTime / (flashDuration / 2));
            float newColorB = Mathf.Lerp(_originalColor.b, 255.0f, elapsedTime / (flashDuration / 2));

            _spriteRenderer.color = new Color(newColorR, newColorG, newColorB, _originalColor.a);

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

            _spriteRenderer.color = new Color(newColorR, newColorG, newColorB, _originalColor.a);

            // All this does is wait until the next frame call
            // The reason this is an IEnumerator is so that we don't have to stick update where it's not needed
            yield return null;
        }
    }
}
