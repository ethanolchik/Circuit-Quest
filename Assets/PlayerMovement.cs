using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkingSpeed = 8f;
    public float jumpPower = 16f;
    private float horizontal;
    public float sprintingSpeed = 12f;
    private float movementSpeed;

    private bool doubleJump = false;

    private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movementSpeed = walkingSpeed;
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");

        Jump();
        Sprint();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(horizontal * movementSpeed, rb.velocity.y);
    }

    // void Flip()
    // {
    //     if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
    //     {
    //         isFacingRight = !isFacingRight;
            
    //         Vector3 localScale = transform.localScale;
    //         localScale.x *= -1f;
    //         transform.localScale = localScale;
    //     }
    // }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if (Input.GetButtonDown("Jump") && !IsGrounded() && !doubleJump)
        {
            doubleJump = true;
            rb.velocity = new Vector2(rb.velocity.x, (jumpPower / 2) + 4);
        }
    }

    void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = sprintingSpeed;
        }
        else
        {
            movementSpeed = walkingSpeed;
        }
    }

    bool IsGrounded()
    {
        if (Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer))
        {
            doubleJump = false;
            return true;
        } 
        return false;
    }

    // public void JumpPadTrigger()
    // {
    //     doubleJump = false;
    //     rb.velocity = new Vector2(rb.velocity.x, jumpPadPower);
    // }
}
