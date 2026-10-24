using System.Collections;
using UnityEngine;

public class FlashAndShakeOnHit : MonoBehaviour
{
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startPosition = transform.position;
        _isShaking = false;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
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
        if (other.CompareTag("DamageSpell"))
        {
            // Object isn't destroyed from the damage, make it shake and flash white
            _startPosition = transform.position; //Update current position before changing it through shaking
            _isShaking = true;
            Invoke(nameof(ResetShake), shakeAmount);
            Invoke(nameof(TriggerFlash), 0.0f);
        }
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
