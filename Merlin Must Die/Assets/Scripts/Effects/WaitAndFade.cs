using Unity.VisualScripting;
using UnityEngine;

public class WaitAndFade : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private float _waitTime;
    [SerializeField] private float _fadeTime;

    private float _waitTimer;
    private float _fadeTimer;

    private bool _fading = false;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!_fading)
        {
            _waitTimer += Time.deltaTime;

            if(_waitTimer > _waitTime)
            {
                _fading = true;
            }
        }
        if (_fading)
        {
            _fadeTimer += Time.deltaTime;
            Color newColor = _spriteRenderer.color;
            newColor.a = (1.0f - (_fadeTimer / _fadeTime));
            _spriteRenderer.color = newColor;
        }     
    }
}
