using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public GameObject tilePrefab;
    public float spawnInterval = 6f;
    public float gridTop = 20f; // Adjust this to the top row of your grid

    void Start()
    {
        InvokeRepeating(nameof(SpawnTile), 1f, spawnInterval);
    }

    void SpawnTile()
    {
        if (tilePrefab != null)
        {
            int randomColumn = Random.Range(-4, 6);
            Debug.Log("Random Column: " + randomColumn);  // Log the random column

            Vector2 spawnPosition = new Vector2(randomColumn, gridTop);
            Debug.Log("Spawn Position: " + spawnPosition);  // Log the final spawn position

            Instantiate(tilePrefab, spawnPosition, Quaternion.identity);
        }
    }
}