using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float groundCheckDistance = 0.15f;
    public bool readKeyboard = false;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private LayerMask groundLayerMask;

    private float moveDirection = 0f; // -1 for left, 1 for right, 0 for idle

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        int thisLayer = gameObject.layer;
        groundLayerMask = ~(1 << thisLayer);
    }

    private void Update()
    {
        HandleHorizontalMovement();
        if(readKeyboard) KeyboardInputs();
    }

    private void HandleHorizontalMovement()
    {
        if (moveDirection != 0f)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveDirection), 1f, 1f);
            transform.position += new Vector3(moveDirection * moveSpeed * Time.deltaTime, 0f, 0f);
        }
        
    }

    public void MoveLeft()
    {
        moveDirection = -1f;
    }

    public void MoveRight()
    {
        moveDirection = 1f;
    }

    public void StopHorizontalMovement()
    {
        moveDirection = 0f;
    }

    public void Jump()
    {
        Vector2 origin = transform.position;
        float rayLength = boxCollider.bounds.extents.y + groundCheckDistance;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, groundLayerMask);
        Debug.DrawRay(origin, Vector2.down * rayLength, Color.red, 1f);

        if (hit.collider && hit.collider.gameObject != gameObject)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
    
    void KeyboardInputs()
    {
        if (Keyboard.current.aKey.isPressed)
        {
            MoveLeft();
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            MoveRight();
        }
        else
        {
            StopHorizontalMovement();
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Jump();
        }
    }
}