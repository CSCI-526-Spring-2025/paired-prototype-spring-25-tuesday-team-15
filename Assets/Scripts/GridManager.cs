using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    private Dictionary<Vector2, string> occupiedPositions = new Dictionary<Vector2, string>();

    private HashSet<string> wordDictionary = new HashSet<string>();

    public bool isGameOver = false;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadWordDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Load Words from Dictionary
    private void LoadWordDictionary()
    {
        TextAsset wordFile = Resources.Load<TextAsset>("words");
        
        if (wordFile != null)
        {
            string[] words = wordFile.text.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            wordDictionary = new HashSet<string>(words);
            foreach (string word in words)
            {
                wordDictionary.Add(word.Trim().ToUpper());
            }
        }
    }

    public bool IsOccupied(Vector2 position)
    {
        return occupiedPositions.ContainsKey(position);
    }

    public void RegisterTile(Vector2 position, string letter)
    {
        if (isGameOver)
            return;

        if (position.y >= 10)
        {
            TriggerGameOver();
            return;
        }

        if (!occupiedPositions.ContainsKey(position))
        {
            occupiedPositions[position] = letter;
            CheckForWords();
        }
    }

    public void UnregisterTile(Vector2 position)
    {
        if (occupiedPositions.ContainsKey(position))
        {
            occupiedPositions.Remove(position);
        }
    }

    // Game Over Condition
    public void TriggerGameOver()
    {
        Debug.LogError("GAME OVER! The grid is full.");
        isGameOver = true;

        TileSpawner.Instance.StopSpawning();
    }


    private void CheckForWords()
    {
        List<Vector2> tilesToClear = new List<Vector2>();

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

        for (int x = -4; x < 6; x++)
        {
            string currentWord = "";
            List<Vector2> currentPositions = new List<Vector2>();

            for (int y = 10; y >= -9; y--)
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
            ClearTiles(tilesToClear);
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
                List<Vector2> matchedPositions = positions.GetRange(startIndex, word.Length);
                matchedTiles.AddRange(matchedPositions);
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
                GameObject tileToDestroy = FindTileAtPosition(pos);
                if (tileToDestroy != null)
                {
                    Destroy(tileToDestroy);
                }

                occupiedPositions.Remove(pos);
            }
        }
        ApplyGravity();
    }

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
        Dictionary<Vector2, string> newGridState = new Dictionary<Vector2, string>();

        for (int x = -4; x < 6; x++)
        {
            List<KeyValuePair<Vector2, string>> columnTiles = new List<KeyValuePair<Vector2, string>>();

            for (int y = -9; y <= 10; y++)
            {
                Vector2 pos = new Vector2(x, y);
                if (occupiedPositions.ContainsKey(pos))
                {
                    columnTiles.Add(new KeyValuePair<Vector2, string>(pos, occupiedPositions[pos]));
                }
            }

            int newY = -9;
            foreach (var tile in columnTiles)
            {
                Vector2 newPos = new Vector2(x, newY);
                GameObject tileObj = FindTileAtPosition(tile.Key);
                if (tileObj != null)
                {
                    tileObj.transform.position = newPos;
                }
                newGridState[newPos] = tile.Value;
                newY++;
            }
        }
        occupiedPositions = newGridState;
    }
}
