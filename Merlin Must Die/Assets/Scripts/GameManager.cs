using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set; }
    //public static bool Paused { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        //If there is already a game manager in the scene destroy it
        //(Probably for running levels individually)
        else
        {
            Destroy(gameObject);
        }
    }
}
