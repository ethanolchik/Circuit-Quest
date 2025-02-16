using UnityEngine;

public class ShowHealthSystem : MonoBehaviour
{
    [SerializeField] private GameObject healthbar;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 6)
        {
            // Enable the healthbar image
            healthbar.GetComponent<SpriteRenderer>().enabled = true;
            HealthSystem.Instance.UnlockHealthbar();

            // Destroy the object as it prevents the player from interacting with the enemy
            Destroy(gameObject);
        }
    }
}
