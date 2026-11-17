using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton class that manages and updates the Healthbar UI
/// </summary>
public class Healthbar : MonoBehaviour
{
    public static Healthbar Instance { get; private set; }

    /// <summary>
    /// All active chunks in the scene
    /// </summary>
    private List<Image> _activeChunks = new List<Image>();

    /// <summary>
    /// Represents a "chunk" or segment of the healthbar
    /// </summary>
    [SerializeField]
    private Image _healthChunk;

    private void Awake()
    {
        // If an instance already exists, destroy this duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set the global instance and protect it from scene changes
        Instance = this;
        // DontDestroyOnLoad(gameObject); <-- Uncomment if we want the healthbar to persist across scenes
    }

    /// <summary>
    /// Clears the health bar and instantiates the new number of health
    /// </summary>
    /// <param name="numChunksToSpawn"></param>
    public void UpdateHealthbar(int numChunksToSpawn)
    {
        if (_activeChunks.Count > 0)
        {
            ClearChunks();
        }
        for (int i = 0; i < numChunksToSpawn; i++)
        {
            Image newChunk = Instantiate(_healthChunk, transform);
            newChunk.transform.Translate(new Vector3(0f, -i, 0f));
            
            _activeChunks.Add(newChunk);
        }
    }

    /// <summary>
    /// Clears the scene of the active chunks and resets the active chunk list
    /// </summary>
    private void ClearChunks()
    {
        foreach(Image i in _activeChunks)
        {
            Destroy(i.gameObject);
        }
        _activeChunks.Clear();
    }
}
