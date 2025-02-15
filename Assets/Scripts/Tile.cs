using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    private static string alphabet = "DOG";
    private SpriteRenderer spriteRenderer;
    private TextMeshProUGUI letterText;
    private bool isPlaced = false; // Flag to check if tile has landed

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        letterText = GetComponentInChildren<TextMeshProUGUI>(); // Auto-detects LetterText

        AssignRandomLetter();
        AssignRandomColor();
    }

    void AssignRandomLetter()
    {
        char randomLetter = alphabet[Random.Range(0, alphabet.Length)];
        letterText.text = randomLetter.ToString();
    }

    void AssignRandomColor()
    {
        spriteRenderer.color = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), 1f);
    }

    // When tile lands, snap it to the grid and update GridManager
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isPlaced && (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Tile")))
        {
            isPlaced = true; // Mark as placed to avoid multiple registrations
            SnapToGrid();
            GridManager.Instance.AddTileToGrid(this); // Register tile in grid
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static; // Stop movement
        }
    }

    // Snap tile to grid position
    private void SnapToGrid()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
        transform.position = position;
    }

    public string GetLetter()
{
    return letterText.text;
}

}