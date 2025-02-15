using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 2f;
    private float moveDelay = 0.2f;
    private float moveTimer;
    private bool isActive = true;  // Flag to check if tile is active

    private void Update()
    {
        if (isActive)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        if (Time.time - moveTimer > moveDelay)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Move(Vector2.left);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                Move(Vector2.right);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                Move(Vector2.down);
            }
        }
    }

    private void Move(Vector2 direction)
    {
        Vector3 newPosition = transform.position + (Vector3)direction;

        if (IsValidMove(newPosition))
        {
            transform.position = newPosition;
            moveTimer = Time.time;
        }
    }

    private bool IsValidMove(Vector3 position)
    {
        // Check boundaries
        if (position.x < -4.5f || position.x > 5f || position.y < -10f)
        {
            return false;
        }

        // Check for collisions
        Collider2D hit = Physics2D.OverlapBox(position, Vector2.one * 0.9f, 0f);
        if (hit != null && hit.gameObject != this.gameObject)
        {
            return false;
        }

        return true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Snap to nearest grid cell
        Vector3 position = transform.position;
        position.x = Mathf.Round(position.x);  // Align X to nearest integer
        position.y = Mathf.Round(position.y);  // Align Y to nearest integer
        transform.position = position;

        // Deactivate controls once tile touches something
        isActive = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static; // Set tile as placed
    }
}