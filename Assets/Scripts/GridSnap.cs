using UnityEngine;

public class GridSnap : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Tile"))
        {
            Vector2 newPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
            transform.position = newPosition;
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }
}