using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkingSpeed = 8f;
    public float jumpPower = 16f;
    private float horizontal;
    public float sprintingSpeed = 12f;
    private float movementSpeed;
    
    public float jumpPadPower = 20f;

    private bool doubleJump = false;

    private bool canTakeDamage = true;

    private bool jumpingOnJumpPad = false;

    private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Sprite sprite;
    [SerializeField] private Sprite dimmedSprite;

    private bool isFacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movementSpeed = walkingSpeed;

        GetComponentInChildren<SpriteRenderer>().sprite = sprite;

        // Set the position of the player to the last checkpoint
        transform.position = HealthSystem.Instance.GetLastCheckpoint();
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");

        // if the player is dimmed they should not be able to take damage.
        if (GetComponentInChildren<SpriteRenderer>().sprite == dimmedSprite)
        {
            canTakeDamage = false;
        } else {
            canTakeDamage = true;
        }

        Jump();
        Sprint();
        Flip();
    }

    public bool CanTakeDamage()
    {
        return canTakeDamage;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(horizontal * movementSpeed, rb.velocity.y);
    }

    public void Dim()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = dimmedSprite;
    }

    public void Undim() {
        GetComponentInChildren<SpriteRenderer>().sprite = sprite;
    }

    void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if (Input.GetButtonDown("Jump") && !IsGrounded() && !doubleJump && !jumpingOnJumpPad)
        {
            doubleJump = true;
            rb.velocity = new Vector2(rb.velocity.x, (jumpPower / 2) + 4);
        }
    }

    public void JumpOnKill()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpPower / 2);
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
            jumpingOnJumpPad = false;
            doubleJump = false;
            return true;
        } 
        return false;
    }

    public void JumpPadTrigger()
    {
        jumpingOnJumpPad = true;
        doubleJump = false;
        rb.velocity = new Vector2(rb.velocity.x, jumpPadPower);
    }
}
