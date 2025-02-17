using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    private Dictionary<Vector2, string> occupiedPositions = new Dictionary<Vector2, string>(); // Store letter with position

    private HashSet<string> wordDictionary = new HashSet<string>();

    // List<Vector2> wordPositions = new List<Vector2>(); // To store positions of found words
    public bool isGameOver = false;  // Track game over state


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("GridManager Initialized!");
            LoadWordDictionary(); // Load words from file
        }
        else
        {
            Debug.LogError("Multiple instances of GridManager detected!");
            Destroy(gameObject);
        }
    }

    // **Load Words from words.txt**
    private void LoadWordDictionary()
    {
        TextAsset wordFile = Resources.Load<TextAsset>("words");
        
        if (wordFile != null)
        {
            string[] words = wordFile.text.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            wordDictionary = new HashSet<string>(words);
            Debug.Log($"📖 Loaded {wordDictionary.Count} words into the dictionary!");
        }
        else
        {
            Debug.LogError("⚠️ Failed to load words.txt! Make sure it is inside the Resources folder.");
        }
    }

    public bool IsOccupied(Vector2 position)
    {
        return occupiedPositions.ContainsKey(position);
    }

    public void RegisterTile(Vector2 position, string letter)
    {
        if (isGameOver)
            return; // Stop if game over

        if (position.y >= 10)
        {
            TriggerGameOver();
            return;
        }

        if (!occupiedPositions.ContainsKey(position))
        {
            occupiedPositions[position] = letter;
            Debug.Log($"✅ Tile '{letter}' placed at {position}");
            PrintGridState();
            CheckForWords();
        }
        else
        {
            Debug.LogWarning($"⚠️ Duplicate registration attempt at {position}");
        }
    }

    public void TriggerGameOver()
    {
        Debug.LogError("GAME OVER! The grid is full.");
        isGameOver = true;

        TileSpawner.Instance.StopSpawning();
    }

    public void UnregisterTile(Vector2 position)
    {
        if (occupiedPositions.ContainsKey(position))
        {
            occupiedPositions.Remove(position);
        }
    }

    private void PrintGridState()
    {
        StringBuilder gridOutput = new StringBuilder("\n📌 **Current Grid State:**\n");

        for (int y = 10; y >= -9; y--) // From top to bottom
        {
            for (int x = -4; x < 6; x++) // From left to right
            {
                Vector2 checkPosition = new Vector2(x, y);
                if (occupiedPositions.ContainsKey(checkPosition))
                {
                    gridOutput.Append($"[{occupiedPositions[checkPosition]}] "); // Show letter
                }
                else
                {
                    gridOutput.Append("[ ] "); // Empty space
                }
            }
            gridOutput.AppendLine(); // New row
        }

        Debug.Log(gridOutput.ToString()); // Print structured grid
    }


    private void CheckForWords()
    {
        Debug.Log("🔎 Checking for words in the grid...");
        List<Vector2> tilesToClear = new List<Vector2>();

        // **Check Rows (Left to Right)**
        for (int y = -9; y <= 10; y++)
        {
            string currentWord = "";
            List<Vector2> currentPositions = new List<Vector2>();

            for (int x = -4; x < 6; x++)
            {
                Vector2 position = new Vector2(x, y);

                if (occupiedPositions.ContainsKey(position))
                {
                    currentWord += occupiedPositions[position];
                    currentPositions.Add(position);
                }
                else
                {
                    tilesToClear.AddRange(CheckWordMatch(currentWord, currentPositions));
                    currentWord = "";
                    currentPositions.Clear();
                }
            }
            tilesToClear.AddRange(CheckWordMatch(currentWord, currentPositions));
        }

        // **Check Columns (Top to Bottom)**
        for (int x = -4; x < 6; x++)
        {
            string currentWord = "";
            List<Vector2> currentPositions = new List<Vector2>();

            for (int y = 10; y >= -9; y--)  // Start from top row
            {
                Vector2 position = new Vector2(x, y);

                if (occupiedPositions.ContainsKey(position))
                {
                    currentWord += occupiedPositions[position];
                    currentPositions.Add(position);
                }
                else
                {
                    tilesToClear.AddRange(CheckWordMatch(currentWord, currentPositions));
                    currentWord = "";
                    currentPositions.Clear();
                }
            }
            tilesToClear.AddRange(CheckWordMatch(currentWord, currentPositions));
        }

        if (tilesToClear.Count > 0)
        {
            Debug.Log($"🔴 Words found! Clearing {tilesToClear.Count} tiles...");
            ClearTiles(tilesToClear);
        }
        else
        {
            Debug.Log("No words found.");
        }
    }

    private List<Vector2> CheckWordMatch(string rowString, List<Vector2> positions)
    {
        List<Vector2> matchedTiles = new List<Vector2>();

        foreach (string word in wordDictionary)
        {
            int startIndex = rowString.IndexOf(word);
            while (startIndex != -1)
            {
                // **Ensure only the word's exact range is selected**
                List<Vector2> matchedPositions = positions.GetRange(startIndex, word.Length);
                matchedTiles.AddRange(matchedPositions);
                Debug.Log($"✅ Word Found: {word} at positions {string.Join(", ", matchedPositions)}");

                // **Continue checking for multiple occurrences in the same row**
                startIndex = rowString.IndexOf(word, startIndex + 1);
            }
        }

        return matchedTiles;
    }

    private void ClearTiles(List<Vector2> tilesToClear)
    {
        foreach (Vector2 pos in tilesToClear)
        {
            if (occupiedPositions.ContainsKey(pos))
            {
                // **Destroy the actual GameObject**
                GameObject tileToDestroy = FindTileAtPosition(pos);
                if (tileToDestroy != null)
                {
                    Destroy(tileToDestroy);
                }

                // **Remove from grid data**
                occupiedPositions.Remove(pos);
            }
        }

        Debug.Log("✅ Tiles cleared. Applying gravity...");
        ApplyGravity();
    }

    // **Helper function to find a tile at a position**
    private GameObject FindTileAtPosition(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(position);
        foreach (Collider2D col in colliders)
        {
            if (col.gameObject.CompareTag("Tile"))
            {
                return col.gameObject;
            }
        }
        return null;
    }

    private void ApplyGravity()
    {
        Debug.Log("🔽 Applying Gravity...");

        Dictionary<Vector2, string> newGridState = new Dictionary<Vector2, string>();
        Dictionary<Vector2, GameObject> newTileObjects = new Dictionary<Vector2, GameObject>();

        for (int x = -4; x < 6; x++) // **Iterate column-wise**
        {
            List<KeyValuePair<Vector2, string>> columnTiles = new List<KeyValuePair<Vector2, string>>();

            for (int y = -9; y <= 10; y++) // **Collect existing tiles in the column**
            {
                Vector2 pos = new Vector2(x, y);
                if (occupiedPositions.ContainsKey(pos))
                {
                    columnTiles.Add(new KeyValuePair<Vector2, string>(pos, occupiedPositions[pos]));
                }
            }

            int newY = -9; // Start placing at the lowest row
            foreach (var tile in columnTiles)
            {
                Vector2 newPos = new Vector2(x, newY);

                // **Move tile object in scene**
                GameObject tileObj = FindTileAtPosition(tile.Key);
                if (tileObj != null)
                {
                    tileObj.transform.position = newPos;
                }

                // **Update occupiedPositions**
                newGridState[newPos] = tile.Value;
                newY++; // Move upwards for next tile
            }
        }

        // **Replace with new adjusted state**
        occupiedPositions = newGridState;
        Debug.Log("✅ Gravity Applied.");
        PrintGridState();
    }
}
