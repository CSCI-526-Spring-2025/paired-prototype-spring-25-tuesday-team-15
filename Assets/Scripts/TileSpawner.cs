using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public GameObject tilePrefab;
    public float gridTop = 10f; // Adjust this to the top row of your grid

    private GameObject currentTile;

    void Start()
    {
        SpawnTile();
    }

    // Spawns a new tile at a random column at the top of the grid
    void SpawnTile()
    {
        if (tilePrefab != null)
        {
            int randomColumn = Random.Range(-4, 6);
            // Debug.Log("Random Column: " + randomColumn);

            Vector2 spawnPosition = new Vector2(randomColumn, gridTop);
            // Debug.Log("Spawn Position: " + spawnPosition);

            currentTile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);

            // Subscribe to the tile's OnTilePlaced event
            PlayerControl control = currentTile.GetComponent<PlayerControl>();
            if (control != null)
            {
                control.OnTilePlaced += HandleTilePlaced;
            }
        }
    }

    // Called when the current tile has been placed
    void HandleTilePlaced()
    {
        // Unsubscribe to prevent potential memory leaks
        if (currentTile != null)
        {
            PlayerControl control = currentTile.GetComponent<PlayerControl>();
            if (control != null)
            {
                control.OnTilePlaced -= HandleTilePlaced;
            }
        }
        // Spawn the next tile
        SpawnTile();
    }
}