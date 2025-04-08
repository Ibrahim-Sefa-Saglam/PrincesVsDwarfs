using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float groundCheckDistance = 0.15f;

    public Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    // This LayerMask will ignore the "Player" layer
    private LayerMask groundLayerMask;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Get all layers except "Player"
        int thisLayer = gameObject.layer;
        groundLayerMask = ~(1 << thisLayer);
    }
    public void MoveLeft()
    {
        transform.localScale = new Vector3(-1, 1, 1);
        rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
    }

    public void MoveRight()
    {
        transform.localScale = new Vector3(1, 1, 1);
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    public void StopHorizontalMovement()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }
    public void Jump()
    {
        Vector2 origin = transform.position;
        float rayStartOffset = boxCollider.bounds.extents.y;
        float rayLength = rayStartOffset + groundCheckDistance;

        Vector2 rayStart = origin;
        Vector2 rayEnd = origin + Vector2.down * rayLength;

        // Use LayerMask to ignore "Player"
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, rayLength, groundLayerMask);
        Debug.DrawLine(rayStart, rayEnd, Color.red, 1f);

        if (hit.collider)
        {
            if(hit.collider.gameObject == gameObject) return;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
}