using UnityEngine;
using System.Collections; // for IEnumerator

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 2f;
    private float moveDelay = 0.2f;
    private float moveTimer;
    private bool isActive = true;  // Flag to check if tile is active

    // Public property to expose the active status
    public bool IsActiveTile
    {
        get { return isActive; }
    }

    // Event to signal when the tile is placed
    public event System.Action OnTilePlaced;

    private void Update()
    {
        if (isActive)
        {
            HandleMovement();

            // Instead of dropping in the same frame, start a coroutine
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(DropInstantly());
            }
        }
    }

    // Check for movement inputs (arrow keys + A, S, D)
    private void HandleMovement()
    {
        if (Time.time - moveTimer > moveDelay)
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                Move(Vector2.left);
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                Move(Vector2.right);
            }
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                Move(Vector2.down);
            }
        }
    }

    // Coroutine that drops the tile step-by-step until it can no longer move
    private IEnumerator DropInstantly()
    {
        // Disable normal movement so we don't conflict with the coroutine
        isActive = false;

        // Keep moving the tile down one unit each frame while valid
        while (IsValidMove(transform.position + Vector3.down))
        {
            transform.position += Vector3.down;

            // Let physics detect the new position
            // so we don't "skip" collisions.
            Physics2D.SyncTransforms();

            // Wait 1 frame before the next downward step
            yield return null;
        }

        // Snap the tile to the grid and place it
        SnapToGrid();
        PlaceTile();
    }

    // Attempts to move the tile in the given direction if the move is valid
    private void Move(Vector2 direction)
    {
        Vector3 newPosition = transform.position + (Vector3)direction;

        if (IsValidMove(newPosition))
        {
            transform.position = newPosition;
            moveTimer = Time.time;
        }
    }

    // Checks boundaries and collision using an overlap box
    private bool IsValidMove(Vector3 position)
    {
        // Check boundaries
        if (position.x < -4.5f || position.x > 5f || position.y < -10f)
        {
            return false;
        }

        // Check for collisions with other objects
        Collider2D hit = Physics2D.OverlapBox(position, Vector2.one * 0.9f, 0f);
        if (hit != null && hit.gameObject != this.gameObject)
        {
            return false;
        }

        return true;
    }

    // Snaps the tile's position to the nearest grid cell
    private void SnapToGrid()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
        transform.position = position;
    }

    // Deactivates tile movement and sets the rigidbody to Static
    private void PlaceTile()
    {
        // Mark inactive so it no longer responds to input
        isActive = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        // Fire the placement event
        OnTilePlaced?.Invoke();
    }

    // When a collision is detected, snap to grid and place the tile
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActive) // Only do this if we're still active (normal fall)
        {
            SnapToGrid();
            PlaceTile();
        }
    }
}