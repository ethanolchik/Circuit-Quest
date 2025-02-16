using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfLevel : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // If the player is colliding
        if (other.gameObject.layer == 6)
        {
            // Reset the player's position
            HealthSystem.Instance.SetLastCheckpoint(Vector3.zero);

            // Reset the bridges JSON file
            BridgeManager.Instance.ResetIndex();

            // Unlock the next level and load the level select screen
            GameInfo.Instance.NextLevel();
            SceneManager.LoadSceneAsync("Scenes/LevelSelect");
        }
    }
}
