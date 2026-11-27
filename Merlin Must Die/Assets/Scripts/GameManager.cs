using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    //public static bool Paused { get; private set; }
    public static Player Player { get; private set; }
    public static Grimoire Grimoire { get; private set; }
    public static Excalibur Excalibur { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            GameManager.Player = GetComponentInChildren<Player>();
            GameManager.Grimoire = GetComponentInChildren<Grimoire>();
            GameManager.Excalibur = GetComponentInChildren<Excalibur>();

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
