using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public static TileSpawner Instance { get; internal set; }

    public GameObject tilePrefab;

    public float gridTop = 10f;

    private GameObject currentTile;

    private bool canSpawn = true;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        SpawnTile();
    }

    public void SpawnTile()
    {
        if (!canSpawn || GridManager.Instance.isGameOver) return; 

        if (tilePrefab != null)
        {
            int randomColumn = Random.Range(-4, 6);
            Vector2 spawnPosition = new Vector2(randomColumn, gridTop);

            if (GridManager.Instance.IsOccupied(spawnPosition))
            {
                GridManager.Instance.TriggerGameOver();
                return;
            }

            currentTile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);

            PlayerControl control = currentTile.GetComponent<PlayerControl>();
            if (control != null)
            {
                control.OnTilePlaced += HandleTilePlaced;
            }
        }
    }

    public void StopSpawning()
    {
        canSpawn = false;
    }

    private void HandleTilePlaced()
    {
        if (currentTile != null)
        {
            PlayerControl control = currentTile.GetComponent<PlayerControl>();
            if (control != null)
            {
                control.OnTilePlaced -= HandleTilePlaced;
            }
        }
        SpawnTile();
    }
}