
using Mono.Cecil.Cil;
using UnityEngine;

public class LightningBoltObject : SpellObject
{
    // TODO: When animation is done playing, delete this object instead of having timer logic
    // We will need the animated sprite/particle effect first

    private Animator _animator;
    private Player _player;
    private float _originalPlayerMaxSpeed;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        _player = FindAnyObjectByType<Player>();
        _originalPlayerMaxSpeed = _player.MaxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("LightningBoltAnimation"))
        {
            // Lightning bolt animation is not playing, play it
            _animator.Play("LightningBoltAnimation");
        }
        else if (stateInfo.normalizedTime < 1.0f)
        {
            // Keep player locked in place
            _player.RB.linearVelocity = Vector2.zero;
            _player.MaxSpeed = 0.0f;
        }
        else
        {
            // Check to see if the animation is done playing
            // When it is, delete this object
            _player.MaxSpeed = _originalPlayerMaxSpeed;
            Destroy(gameObject);
        }
    }
}
