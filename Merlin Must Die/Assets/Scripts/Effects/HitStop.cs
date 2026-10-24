using System.Collections;
using UnityEngine;

/// <summary>
/// Adds a GLOBAL hitstop effect when called. SHOULD BE USED SPARINGLY
/// </summary>
public class HitStop : MonoBehaviour
{
    private bool _waiting = false;

    /// <summary>
    /// How long the HitStop effect lasts for
    /// </summary>
    [SerializeField]
    private float _duration = 0.1f;

    public void Stop()
    {
        if (_waiting) return;
        StartCoroutine(WaitHitStop());
    }

    private IEnumerator WaitHitStop()
    {
        _waiting = true;
        Time.timeScale = 0.0f;

        yield return new WaitForSecondsRealtime(_duration);

        Time.timeScale = 1.0f;
        _waiting = false;
    }
}
