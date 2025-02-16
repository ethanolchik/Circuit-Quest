using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if its the player colliding
        if (other.gameObject.layer == 6)
        {
            // Set the last checkpoint to the checkpoint's position
            HealthSystem.Instance.SetLastCheckpoint(transform.position);
        }
    }
}
