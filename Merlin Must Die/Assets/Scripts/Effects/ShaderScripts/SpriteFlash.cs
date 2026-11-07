using UnityEngine;

/// <summary>
/// Produces an effect where a sprite will flash over a certain amount of time
/// </summary>
public class SpriteFlash : MonoBehaviour
{
    [SerializeField] private float flashDecaySpeed = 1.0f;

    private SpriteRenderer[] _spriteRenderers;
    private MaterialPropertyBlock _propertyBlock;
    private float _blinkFactor;

    private void Start()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (_blinkFactor <= 0.0f)
        {
            return;
        }
        _blinkFactor = Mathf.Lerp(_blinkFactor, 0.0f, Time.deltaTime * flashDecaySpeed);
        
        // Think of this as an epsilon value
        if (_blinkFactor <= 0.01f)
        {
            _blinkFactor = 0.0f;
        }
        ApplyBlinkFactor();
    }

    public void Blink()
    {
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
}
