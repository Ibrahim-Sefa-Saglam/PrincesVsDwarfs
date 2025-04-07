using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float groundCheckDistance = 0.1f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool jumpPressed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Handle left/right turning and movement
        if (moveInput.x < 0)
            MoveLeft();
        else if (moveInput.x > 0)
            MoveRight();

        // Handle jump input
        if (jumpPressed)
        {
            Jump();
            jumpPressed = false; // Prevent continuous jumping
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpPressed = true;
        }
    }

    void MoveLeft()
    {
        transform.localScale = new Vector3(-1, 1, 1); // Face left
        rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
    }

    void MoveRight()
    {
        transform.localScale = new Vector3(1, 1, 1); // Face right
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    void Jump()
    {
        Vector2 origin = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance + 0.1f);

        Debug.DrawRay(origin, Vector2.down * (groundCheckDistance + 0.1f), Color.red, 0.1f);

        if (hit.collider != null)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
}