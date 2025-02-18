using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    private TextMeshProUGUI letterText;
    private static string alphabet = 
        "AAAAA"    // 5 A's
    + "EEEE"   // 4 E's
    + "III"     // 3 I's
    + "OOO"    // 3 O's
    + "UUU"       // 3 U's
    + "BBCCDDFFGGHHJJKKLLMMMNNPPPQRRSSTTVWWXYZ";
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