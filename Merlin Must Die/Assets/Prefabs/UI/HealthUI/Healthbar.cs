using NUnit.Framework;
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
    /// Spawn location for the first chunk of the healthbar. All other chunks are spawned based off this location
    /// </summary>
    private Vector2 _chunk1SpawnLocation = new Vector2(900.0f, -425.0f);

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

    public void UpdateHealthbar(int numChunksToSpawn)
    {
        if (_activeChunks.Count > 0)
        {
            ClearChunks();
        }
        for (int i = 0; i < numChunksToSpawn; i++)
        {
            Vector2 spawnLocation = _chunk1SpawnLocation + new Vector2(0.0f, _healthChunk.rectTransform.rect.height * i); // Adjust the offset as needed
            Image newChunk = Instantiate(_healthChunk, spawnLocation, Quaternion.identity);
            newChunk.transform.SetParent(GetComponentInParent<Canvas>().transform, false); // Set the parent to the Canvas to ensure proper rendering
            _activeChunks.Add(newChunk);
        }
    }

    private void ClearChunks()
    {
        foreach(Image i in _activeChunks)
        {
            Destroy(i.gameObject);
        }
        _activeChunks.Clear();
    }
}
