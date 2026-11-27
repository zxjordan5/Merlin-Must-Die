using UnityEngine;

/// <summary>
/// Plays an explosion animation and then deletes the explosion GameObject after the animation is completed.
/// </summary>
public class Explosion : MonoBehaviour
{
    /// <summary>
    /// Animator component on the object
    /// </summary>
    private Animator _animator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("ExplosionAnimation"))
        {
            // Lightning bolt animation is not playing, play it
            _animator.Play("ExplosionAnimation");
        }
        else if (stateInfo.normalizedTime > 0.9f)
        {
            _animator.StopPlayback();
            Destroy(gameObject);
        }
    }
}
