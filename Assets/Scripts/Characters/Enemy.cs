using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    /// <summary>
    /// The speed of the enemy
    /// </summary>
    private float speed = 6f;
    /// <summary>
    /// Used to store information about the sprite including the direction it is facing
    /// </summary>
    protected SpriteRenderer sprite;
    /// <summary>
    /// The layer which the script uses to determine whether or not to swap movement direction.
    /// </summary>
    private LayerMask endOfPlatform;

    private Rigidbody2D rb;
    [SerializeField] private Collider2D other;

    private bool canTakeDamage = true;

    protected bool shouldFreeze = false;

    /// <summary>
    /// Set for each invdividual enemy type
    /// </summary>
    protected virtual float height { get { return 0.5f; } }

    public void Kill()
    {
        Destroy(gameObject);
    }

    public void Flip()
    {
        sprite.flipX = !sprite.flipX;
    }

    protected void Move()
    {
        // Check if the enemy is stunned
        if (shouldFreeze)
            return;
        if (sprite.flipX)
            transform.Translate(Time.deltaTime * speed * transform.right);
        else
            transform.Translate(Time.deltaTime * -speed * transform.right);
    }

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();

        endOfPlatform = LayerMask.GetMask("EndOfPlatform");
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void FixedUpdate()
    {
        // check if there is a collision
        if (rb.IsTouching(other) && canTakeDamage) {
            if (other.transform.position.y - 0.5f > transform.position.y + height) {
                // The player is above the enemy
                Destroy(gameObject);
                other.gameObject.GetComponentInParent<PlayerMovement>().JumpOnKill();
            } else {
                HealthSystem.Instance.Damage();
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        canTakeDamage = false;
    }

    void OnTriggerExit2D(Collider2D other) {
        canTakeDamage = true;
    }
}
