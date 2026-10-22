using UnityEngine;

public class UISetup : MonoBehaviour
{
    private Canvas _canvas;
    private Camera _camera;

    /// <summary>
    /// Default number of healthbar chunks to be created on scene startup
    /// </summary>
    [SerializeField]
    private int _defaultNumChunks = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set up camera
        _canvas = GetComponent<Canvas>();
        _camera = FindAnyObjectByType<Camera>();
        _canvas.renderMode = RenderMode.ScreenSpaceCamera;
        _canvas.worldCamera = _camera;

        Healthbar.Instance.UpdateHealthbar(_defaultNumChunks);
    }
}
