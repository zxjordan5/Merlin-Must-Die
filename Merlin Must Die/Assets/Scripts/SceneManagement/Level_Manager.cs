using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

public class Level_Manager : MonoBehaviour
{
    [SerializeField] public GameObject enemyParent;
    //[SerializeField] public GameObject LevelEndBox;

    private GameObject _grimoireUi;
    private GameObject _proceedTextPrefab;
    private GameObject _proceedText;
    private int _levelNum;
    
    // ***** EVENT SETUP ***** // 
    private void OnEnable()
    {
        _levelNum = 0;
        // Set up enemy death event listeners
        if (enemyParent)
        {
            Wander_Enemy[] enemies = enemyParent.GetComponentsInChildren<Wander_Enemy>();
            foreach (Wander_Enemy enemy in enemies)
            {
                enemy.OnEnemyDeath += EnemyDeath;
            }
        }
        
        _grimoireUi = GameObject.Find("GrimoireUI");
        if (!_grimoireUi) { Debug.LogError("GrimoireUI not found!"); }
        
        StartCoroutine(LoadUIComponents());
    }

    /// <summary>
    /// Event that is called when an enemy dies
    /// </summary>
    private void EnemyDeath(Wander_Enemy enemy)
    {
        // Reset event dependency (needed to prevent errors when replaying levels)
        enemy.OnEnemyDeath -= EnemyDeath;
        Destroy(enemy.gameObject);
        
        StartCoroutine(CheckRemainingChildren());
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // Checks if all enemys are dead before moving onto the next level
        if (enemyParent.transform.childCount == 0)
        {
            switch (_levelNum)
            {
                // 2nd level should lead to the win screen
                case 2:
                    Debug.Log("loading win + " + _levelNum);
                    GlobalSceneManager.Instance.ChangeScene(LevelScenePaths.WinScene);
                    break;
                // 2nd level is the default
                default:
                    Debug.Log("loading lv2 + " + _levelNum);
                    GlobalSceneManager.Instance.ChangeScene(LevelScenePaths.LevelTwo);
                    break;
                // This function is apparently called twice :)
            }
        }

    }

    public void OnClick()
    {
        switch (GlobalSceneManager.Instance.CurrentScene)
        {
            // Main menu is the default
            default:
                Debug.Log("loading lv1 " + _levelNum);
                _levelNum = 0;
                GlobalSceneManager.Instance.ChangeScene(LevelScenePaths.LevelOne); //_levelScenePathsInstance.LevelOne
                break;
            // GameOver & Win screens both lead to the Main menu
            case LevelScenePaths.GameOverScene:
            case LevelScenePaths.WinScene:
                GlobalSceneManager.Instance.ChangeScene(LevelScenePaths.MainMenuScene);
                break;
        }
        
    }

    IEnumerator CheckRemainingChildren()
    {
        // We need to wait for the end of the frame after the child has
        // been deleted because unity caches childCount until the next frame
        yield return new WaitForEndOfFrame();
        
        if (enemyParent.transform.childCount == 0)
        {
            // Enemies are all defeated
            _proceedText = Instantiate(_proceedTextPrefab, _grimoireUi.transform, false);
            _levelNum++;
            StartCoroutine(DestroyTextAfterWait(5.0f));
        }
    }

    IEnumerator LoadUIComponents()
    {
        AsyncOperationHandle<GameObject> handleProceedText = Addressables.LoadAssetAsync<GameObject>("ProceedTextPopup");
        yield return handleProceedText;
        if(handleProceedText.Status == AsyncOperationStatus.Succeeded)
        {
            _proceedTextPrefab = handleProceedText.Result.gameObject;
        }
    }
    
    IEnumerator DestroyTextAfterWait(float delayInSeconds)
    {
        // Pauses the coroutine for the specified number of seconds
        yield return new WaitForSeconds(delayInSeconds);
        Destroy(_proceedText);
    }

    IEnumerator Wait(float delayInSeconds)
    {
        // Pauses the coroutine for the specified number of seconds
        yield return new WaitForSeconds(delayInSeconds);
    }
}


/// <summary>
/// EXAMPLE class for a button that when clicked, loads a scene
/// Keeping it here so I can delete the script without fear
/// </summary>
/*public class StartGameButton : MonoBehaviour
{
    LevelScenePaths _levelScenePathsInstance;

    public void OnClick()
    {
        // MUST create an instance of the LevelScenePaths ScriptableObject to access its data
        _levelScenePathsInstance = ScriptableObject.CreateInstance<LevelScenePaths>();
        GlobalSceneManager.Instance.ChangeScene(LevelScenePaths.LevelOne);
    }
}*/
