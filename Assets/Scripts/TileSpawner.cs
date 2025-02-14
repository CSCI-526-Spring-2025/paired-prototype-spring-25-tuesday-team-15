using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public GameObject tilePrefab; // Reference to Tile Prefab
    public float spawnInterval = 2f; // Time between spawns
    public int gridWidth = 1; // Set this based on your grid size

    void Start()
    {
        InvokeRepeating(nameof(SpawnTile), 1f, spawnInterval); // Auto-spawn tiles
    }

    void SpawnTile()
    {
        if (tilePrefab != null)
        {
            // Generate a random X position within the grid width
            float randomX = Random.Range(-gridWidth / 2, gridWidth / 2) +   0.5f;

            // Spawn at the top of the grid
            Vector2 spawnPosition = new Vector2(randomX, 15); // Adjust '20' based on grid height

            Instantiate(tilePrefab, spawnPosition, Quaternion.identity);
        }
    }
}