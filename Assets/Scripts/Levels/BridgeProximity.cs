using UnityEngine;
using UnityEngine.SceneManagement;

public class BridgeProximity : MonoBehaviour
{
    [SerializeField] private string sceneName;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 6)
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 6)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void DestroyProximityMessage()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (GetComponent<SpriteRenderer>().enabled)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Save the position and tell the game to load the player to this position
                HealthSystem.Instance.SetBridgeCheckpoint(transform.position);
                HealthSystem.Instance.SetBridgePuzzle(true);

                GameInfo.Instance.AddCurrentScene();
                SceneManager.LoadSceneAsync(sceneName);
            }
        }
    }
}
