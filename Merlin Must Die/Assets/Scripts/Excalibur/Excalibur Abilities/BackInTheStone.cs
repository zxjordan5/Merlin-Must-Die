using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BackInTheStone : ExcaliburAbility
{
    private GameObject _initialAOEObj;
    private GameObject _damageField;

    private void Awake()
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>("");
        Addressables.LoadAssetAsync<>()
    }
    
    public override void Activate()
    {
        // TODO: Spawn damage field and delete after some time
        // Have the field do an initial burst of damage, and then deal damage over time
        // Implement a method for enemies where they can take damage over time through OnTriggerStay2D
        
        // TODO: In the future, we will most likely want to lock Morgana's movement until the animation is done playing
    }
}
