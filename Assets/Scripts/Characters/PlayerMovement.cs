using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkingSpeed = 8f;
    public float jumpPower = 16f;
    private float horizontal;
    public float sprintingSpeed = 12f;
    private float movementSpeed;

    private bool doubleJump = false;

    private bool canTakeDamage = true;

    private bool jumpingOnJumpPad = false;

    private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Sprite sprite;
    [SerializeField] private Sprite dimmedSprite;
    [SerializeField] private Sprite runSprite;
    [SerializeField] private Sprite runSpriteDimmed; 

    private float spriteChangeRate = 0.1f;
    private float spriteChangeTimer = 0f;
    private SpriteRenderer spriteRenderer;

    private bool isFacingRight = true;

    private bool start = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        movementSpeed = walkingSpeed;

        spriteRenderer.sprite = sprite;
    }

    void Update()
    {
        if (start)
        {
            start = false;
            // Set the position of the player to the last checkpoint if there wasn't a bridge puzzle
            if (HealthSystem.Instance.GetBridgeCheckpoint() == new Vector2(0, 1))
                transform.position = HealthSystem.Instance.GetLastCheckpoint();
        }

        horizontal = Input.GetAxis("Horizontal");

        // Determine damage state
        if (spriteRenderer.sprite == dimmedSprite || spriteRenderer.sprite == runSpriteDimmed)
        {
            canTakeDamage = false;
        } else {
            canTakeDamage = true;
        }

        Jump();
        Sprint();
        Flip();

        UpdateSprite();
    }

    private void UpdateSprite()
    {
        // velocity -> speed
        float speed = Mathf.Abs(rb.velocity.x);

        if (speed > 0.1f) 
        {
            // The current speed as a fraction of the max speed multiplied by time elapsed
            // This allows for the rate at which the sprite changes to increase based on the speed
            spriteChangeTimer += Time.deltaTime * (speed / movementSpeed);

            if (spriteChangeTimer >= spriteChangeRate)
            {
                spriteChangeTimer = 0f;

                // Toggle between running and idle sprites
                bool isRunningSprite = spriteRenderer.sprite == runSprite || spriteRenderer.sprite == runSpriteDimmed;
                spriteRenderer.sprite = isRunningSprite 
                    ? (canTakeDamage ? sprite : dimmedSprite)
                    : (canTakeDamage ? runSprite : runSpriteDimmed);
            }
        }
        else 
        {
            spriteRenderer.sprite = canTakeDamage ? sprite : dimmedSprite;
            spriteChangeTimer = 0f; // Reset timer when stationary
        }
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
        spriteRenderer.sprite = dimmedSprite;
    }

    public void Undim() {
        spriteRenderer.sprite = sprite;
    }

    void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
        
            SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
            sprite.flipX = !sprite.flipX;
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

    public void JumpPadTrigger(float power)
    {
        jumpingOnJumpPad = true;
        doubleJump = false;
        rb.velocity = new Vector2(rb.velocity.x, power);
    }
}
