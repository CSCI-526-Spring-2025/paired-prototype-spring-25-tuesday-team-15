using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    private TextMeshProUGUI letterText;
    private static string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private SpriteRenderer spriteRenderer;
    private string tileLetter; 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        letterText = GetComponentInChildren<TextMeshProUGUI>(); 

        AssignRandomLetter();
        AssignRandomColor();
    }

    void AssignRandomLetter()
    {
        char randomLetter = alphabet[Random.Range(0, alphabet.Length)];
        tileLetter = randomLetter.ToString();
        letterText.text = tileLetter;
    }

    void AssignRandomColor()
    {
        Color[] tetrisColors = new Color[] {
            Color.cyan, 
            Color.blue, 
            new Color(1f, 0.5f, 0f, 1f), 
            Color.yellow, 
            Color.green, 
            new Color(0.5f, 0f, 0.5f, 1f), 
            Color.red
        };

        spriteRenderer.color = tetrisColors[Random.Range(0, tetrisColors.Length)];
    }

    public string GetTileLetter()
    {
        return tileLetter;
    }
}