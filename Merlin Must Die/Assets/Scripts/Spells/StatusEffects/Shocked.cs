using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Behavioral script for the "Shocked" status effect. This status effect reduces an enemy's speed to zero.
/// </summary>
public class Shocked : StatusEffect
{
    private Vector2 _initialEnemyPos;

    private void Awake()
    {
        StartCoroutine(LoadParticleEffects());
    }
    protected override void Start()
    {
        base.Start();
        effectDuration = 3.0f;
        _initialEnemyPos = _enemy.transform.position;
    }
    // Update is called once per frame
    private void FixedUpdate() 
    {
        if (!_rb) return;
        
        // Lock enemy position and velocity for the effect's duration
        _rb.linearVelocity = Vector2.zero;
        _enemy.transform.position = _initialEnemyPos;
    }
    
    protected override IEnumerator LoadParticleEffects()
    {
        AsyncOperationHandle<GameObject> handleShockedParticles = Addressables.LoadAssetAsync<GameObject>("ShockedParticles");
        yield return handleShockedParticles;
        if (handleShockedParticles.Status == AsyncOperationStatus.Succeeded)
        {
            effectParticlesObj = handleShockedParticles.Result.gameObject;
            Debug.Log(effectParticlesObj.name);
        }

        _instantiatedParticleObjRef = Instantiate(effectParticlesObj, _enemy.transform, false).gameObject;
    }
}
