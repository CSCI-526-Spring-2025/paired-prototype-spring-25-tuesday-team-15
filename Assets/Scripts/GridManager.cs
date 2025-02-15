using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    public int gridWidth = 10;
    public int gridHeight = 20;
    private Tile[,] gridArray;
    private HashSet<string> wordDictionary = new HashSet<string> { "CAT", "DOG", "TREE", "FISH", "STAR" }; // Example words

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        gridArray = new Tile[gridWidth, gridHeight];
    }

    public void AddTileToGrid(Tile tile)
{
    Vector2Int gridPos = GetGridPosition(tile.transform.position);
    if (IsWithinBounds(gridPos))
    {
        gridArray[gridPos.x, gridPos.y] = tile;
        Debug.Log($"✅ Tile {tile.GetLetter()} added at {gridPos.x}, {gridPos.y}"); // 🔥 Debug
        CheckForWords(); // Check for words after placement
    }
}

    private Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y));
    }

    private bool IsWithinBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight;
    }

    // **🚀 Step 2: Check for Words**
    private void CheckForWords()
{
    List<Tile> tilesToDestroy = new List<Tile>();

    // **Check Horizontal Words**
    for (int y = 0; y < gridHeight; y++)
    {
        string word = "";
        List<Tile> currentTiles = new List<Tile>();

        Debug.Log($"Checking Row {y}: "); // Log row number

        for (int x = 0; x < gridWidth; x++)
        {
            if (gridArray[x, y] != null)
            {
                word += gridArray[x, y].GetLetter();
                currentTiles.Add(gridArray[x, y]);
            }
            else
            {
                Debug.Log($"➡️ Row {y} Word: {word}"); // Log found word

                if (word.Length >= 3 && wordDictionary.Contains(word))
                {
                    Debug.Log($"✅ MATCH FOUND: {word} in Row {y}"); // Log successful match
                    tilesToDestroy.AddRange(currentTiles);
                }

                word = "";
                currentTiles.Clear();
            }
        }

        if (word.Length >= 3 && wordDictionary.Contains(word))
        {
            Debug.Log($"✅ MATCH FOUND at end of Row {y}: {word}"); // Log match
            tilesToDestroy.AddRange(currentTiles);
        }
    }

    // **Check Vertical Words**
    for (int x = 0; x < gridWidth; x++)
    {
        string word = "";
        List<Tile> currentTiles = new List<Tile>();

        Debug.Log($"Checking Column {x}: "); // Log column number

        for (int y = 0; y < gridHeight; y++)
        {
            if (gridArray[x, y] != null)
            {
                word += gridArray[x, y].GetLetter();
                currentTiles.Add(gridArray[x, y]);
            }
            else
            {
                Debug.Log($"⬇️ Column {x} Word: {word}"); // Log found word

                if (word.Length >= 3 && wordDictionary.Contains(word))
                {
                    Debug.Log($"✅ MATCH FOUND: {word} in Column {x}"); // Log successful match
                    tilesToDestroy.AddRange(currentTiles);
                }

                word = "";
                currentTiles.Clear();
            }
        }

        if (word.Length >= 3 && wordDictionary.Contains(word))
        {
            Debug.Log($"✅ MATCH FOUND at end of Column {x}: {word}"); // Log match
            tilesToDestroy.AddRange(currentTiles);
        }
    }

    if (tilesToDestroy.Count > 0)
    {
        Debug.Log($"💥 Destroying {tilesToDestroy.Count} tiles!"); // Log number of tiles destroyed
        DestroyMatchedTiles(tilesToDestroy);
    }
    else
    {
        Debug.Log("❌ No words found this round.");
    }
}

    // **🚀 Step 3: Destroy Matched Words**
    private void DestroyMatchedTiles(List<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            Vector2Int gridPos = GetGridPosition(tile.transform.position);
            gridArray[gridPos.x, gridPos.y] = null; // Remove from grid
            Destroy(tile.gameObject); // Destroy the tile
        }

        // Shift remaining tiles downward
        ShiftTilesDown();
    }

    // **🚀 Step 4: Shift Remaining Tiles Down**
    private void ShiftTilesDown()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 1; y < gridHeight; y++) // Start from 1 to avoid bottom row
            {
                if (gridArray[x, y] != null && gridArray[x, y - 1] == null)
                {
                    Tile fallingTile = gridArray[x, y];
                    gridArray[x, y] = null;
                    gridArray[x, y - 1] = fallingTile;

                    fallingTile.transform.position = new Vector3(x, y - 1, 0);
                }
            }
        }
    }
}