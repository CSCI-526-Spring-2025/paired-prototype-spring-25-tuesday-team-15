using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public GameObject tilePrefab;
    public float gridTop = 10f;

    private GameObject currentTile;
    private Dictionary<Vector2, Tile> tileGrid = new Dictionary<Vector2, Tile>();

    private HashSet<string> validWords = new HashSet<string> { "SUN", "CAT", "DOG", "TREE", "CODE" };

    void Start()
    {
        SpawnTile();
    }

    void SpawnTile()
    {
        if (tilePrefab != null)
        {
            int randomColumn = Random.Range(-4, 6);
            Vector2 spawnPosition = new Vector2(randomColumn, gridTop);

            currentTile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);

            PlayerControl control = currentTile.GetComponent<PlayerControl>();
            if (control != null)
            {
                control.OnTilePlaced += HandleTilePlaced;
            }
        }
    }

    void HandleTilePlaced()
    {
        if (currentTile != null)
        {
            Vector2 gridPosition = new Vector2(
                Mathf.Round(currentTile.transform.position.x),
                Mathf.Round(currentTile.transform.position.y)
            );

            StoreTile(gridPosition, currentTile.GetComponent<Tile>());

            PlayerControl control = currentTile.GetComponent<PlayerControl>();
            if (control != null)
            {
                control.OnTilePlaced -= HandleTilePlaced;
            }
        }

        CheckForWords();

        SpawnTile();
    }

    public void StoreTile(Vector2 position, Tile tile)
    {
        if (!tileGrid.ContainsKey(position))
        {
            tileGrid[position] = tile;
            Debug.Log($"Tile stored at {position} with letter: {tile.GetLetter()}");
        }
    }

    void CheckForWords()
    {
        List<Vector2> tilesToClear = new List<Vector2>();

        // **Check all substrings in rows**
        for (int y = -10; y <= 10; y++)
        {
            string currentWord = "";
            List<Vector2> wordPositions = new List<Vector2>();

            for (int x = -4; x <= 5; x++)
            {
                Vector2 pos = new Vector2(x, y);

                if (tileGrid.ContainsKey(pos))
                {
                    currentWord += tileGrid[pos].GetLetter();
                    wordPositions.Add(pos);
                }
                else
                {
                    tilesToClear.AddRange(FindWordsInSequence(currentWord, wordPositions));
                    currentWord = "";
                    wordPositions.Clear();
                }
            }

            tilesToClear.AddRange(FindWordsInSequence(currentWord, wordPositions));
        }

        // **Check all substrings in columns**
        for (int x = -4; x <= 5; x++)
        {
            string currentWord = "";
            List<Vector2> wordPositions = new List<Vector2>();

            for (int y = -10; y <= 10; y++)
            {
                Vector2 pos = new Vector2(x, y);

                if (tileGrid.ContainsKey(pos))
                {
                    currentWord += tileGrid[pos].GetLetter();
                    wordPositions.Add(pos);
                }
                else
                {
                    tilesToClear.AddRange(FindWordsInSequence(currentWord, wordPositions));
                    currentWord = "";
                    wordPositions.Clear();
                }
            }

            tilesToClear.AddRange(FindWordsInSequence(currentWord, wordPositions));
        }

        if (tilesToClear.Count > 0)
        {
            Debug.Log($"🔴 Found {tilesToClear.Count} tiles to clear...");
            ClearTiles(tilesToClear);
        }
        else
        {
            Debug.Log("No words found.");
        }
    }

    List<Vector2> FindWordsInSequence(string sequence, List<Vector2> positions)
    {
        List<Vector2> matchedTiles = new List<Vector2>();

        for (int start = 0; start < sequence.Length; start++)
        {
            for (int length = 3; length <= sequence.Length - start; length++)
            {
                string subWord = sequence.Substring(start, length);
                if (validWords.Contains(subWord))
                {
                    matchedTiles.AddRange(positions.GetRange(start, length));
                    Debug.Log($"✅ Found word: {subWord}");
                }
            }
        }

        return matchedTiles;
    }

    void ClearTiles(List<Vector2> tilesToClear)
    {
        foreach (Vector2 pos in tilesToClear)
        {
            if (tileGrid.ContainsKey(pos))
            {
                Destroy(tileGrid[pos].gameObject);
                tileGrid.Remove(pos);
            }
        }

        ApplyGravity();
    }

    void ApplyGravity()
    {
        Debug.Log("🔽 Applying Gravity...");

        // **Start from second-bottom row and move up**
        for (int y = -9; y <= 10; y++)
        {
            for (int x = -4; x <= 5; x++)
            {
                Vector2 pos = new Vector2(x, y);
                if (tileGrid.ContainsKey(pos))
                {
                    int fallDistance = 0;
                    Vector2 checkPos = new Vector2(x, y - 1);

                    // **Move tile down until it hits another tile or bottom**
                    while (!tileGrid.ContainsKey(checkPos) && checkPos.y > -10)
                    {
                        fallDistance++;
                        checkPos.y--;
                    }

                    if (fallDistance > 0)
                    {
                        Vector2 newPos = new Vector2(x, y - fallDistance);

                        // **Move tile in grid**
                        tileGrid[newPos] = tileGrid[pos];
                        tileGrid.Remove(pos);

                        // **Move tile visually**
                        tileGrid[newPos].transform.position = newPos;
                    }
                }
            }
        }

        Debug.Log("✅ Gravity Applied.");
    }
}