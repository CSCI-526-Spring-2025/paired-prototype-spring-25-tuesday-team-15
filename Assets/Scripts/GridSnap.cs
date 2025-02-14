using UnityEngine;

public class GridSnap : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the tile has landed
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Tile"))
        {
            // Snap the tile to the nearest whole number grid position
            Vector2 newPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
            transform.position = newPosition;

            // Freeze the tile so it doesn’t move anymore
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }
}