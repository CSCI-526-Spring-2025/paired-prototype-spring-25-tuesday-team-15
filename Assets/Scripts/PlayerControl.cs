using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 2f;
    private float moveDelay = 0.2f;
    private float moveTimer;
    private bool isActive = true;  

    public bool IsActiveTile => isActive;
    public event System.Action OnTilePlaced;

    private void Update()
    {
        if (isActive)
        {
            HandleMovement();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(DropInstantly());
            }
        }
    }

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

    private IEnumerator DropInstantly()
    {
        isActive = false;

        while (IsValidMove(transform.position + Vector3.down))
        {
            transform.position += Vector3.down;
            Physics2D.SyncTransforms();
            yield return null;
        }

        SnapToGrid();
        PlaceTile();
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
        if (position.x < -4.5f || position.x > 5f || position.y < -9.5f)
        {
            return false;
        }

        return !GridManager.Instance.IsOccupied(new Vector2(Mathf.Round(position.x), Mathf.Round(position.y)));
    }

    private void SnapToGrid()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
        transform.position = position;
    }

    private void PlaceTile()
    {
        isActive = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        Tile tileComponent = GetComponent<Tile>();
        string tileLetter = tileComponent != null ? tileComponent.GetTileLetter() : "?";

        Vector2 snappedPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
        GridManager.Instance.RegisterTile(snappedPosition, tileLetter);
        OnTilePlaced?.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActive)
        {
            SnapToGrid();
            PlaceTile();
        }
    }
}