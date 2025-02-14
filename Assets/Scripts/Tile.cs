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

    void AssignRandomColor()
    {
        spriteRenderer.color = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), 1f);
    }
}