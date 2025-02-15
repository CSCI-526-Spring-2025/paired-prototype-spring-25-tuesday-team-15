using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    private TextMeshProUGUI letterText;
    private static string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private SpriteRenderer spriteRenderer;

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

    // void AssignRandomColor()
    // {
    //     spriteRenderer.color = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), 1f);
    // }
    void AssignRandomColor()
{
    // I: Cyan, J: Blue, L: Orange, O: Yellow, S: Green, T: Purple, Z: Red
    Color[] tetrisColors = new Color[] {
        Color.cyan,                                  // I
        Color.blue,                                  // J
        new Color(1f, 0.5f, 0f, 1f),                   // L (Orange)
        Color.yellow,                                // O
        Color.green,                                 // S
        new Color(0.5f, 0f, 0.5f, 1f),                 // T (Purple)
        Color.red                                    // Z
    };

    // Randomly select one of the Tetris colors
    spriteRenderer.color = tetrisColors[Random.Range(0, tetrisColors.Length)];
}

}