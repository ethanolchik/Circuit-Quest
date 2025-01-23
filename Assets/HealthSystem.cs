using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HealthSystem : MonoBehaviour
{
    public static HealthSystem Instance;
    [SerializeField] private int hearts;

    /// <summary>
    /// 3 full hearts
    /// </summary>
    [SerializeField] private Sprite heart3;
    /// <summary>
    /// 2 full hearts, 1 empty heart
    /// </summary>
    [SerializeField] private Sprite heart2;
    /// <summary>
    /// 1 full heart, 2 empty hearts
    /// </summary>
    [SerializeField] private Sprite heart1;
    /// <summary>
    /// 3 empty hearts
    /// </summary>
    [SerializeField] private Sprite heart0;

    /// <summary>
    /// The area where the hearts will be shown on the screen
    /// </summary>
    [SerializeField] private Image healthbar;

    [SerializeField] private GameObject player;

    [SerializeField] private Vector2 lastCheckpoint = new Vector2(0, 1);

    void Awake()
    {
        JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(Application.dataPath + "/Resources/HealthSystem.json"), this);
        Instance = this;
    }

    void Start()
    {
        healthbar.sprite = heart3;

        switch (hearts)
        {
            case 2:
                healthbar.sprite = heart2;
                break;
            case 1:
                healthbar.sprite = heart1;
                break;
            case 0:
                healthbar.sprite = heart0;

                // Game over logic here
                Debug.Log("Game Over!");
                break;
        }
    }

    public void Damage()
    {
        if (!player.GetComponent<PlayerMovement>().CanTakeDamage())
        {
            return;
        }

        hearts -= 1;

        switch (hearts)
        {
            case 2:
                healthbar.sprite = heart2;
                break;
            case 1:
                healthbar.sprite = heart1;
                break;
            case 0:
                healthbar.sprite = heart0;

                GameOver();
                break;
        }

        player.GetComponent<PlayerMovement>().Dim();
        StartCoroutine(Wait());
    }

    private void GameOver()
    {
        // Reset the player's health
        hearts = 3;
        healthbar.sprite = heart3;

        int random = Random.Range(0, 4);

        // Randomly load a game-over puzzle scene
        switch (random)
        {
            case 0:
                // Load the first puzzle scene
                break;
            case 1:
                // Load the second puzzle scene
                break;
            case 2:
                // Load the third puzzle scene
                break;
            case 3:
                // Load the fourth puzzle scene
                break;
        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);
        player.GetComponent<PlayerMovement>().Undim();
    }

    void OnDestroy()
    {
        // Clear the file first, then write to it
        System.IO.File.WriteAllText(Application.dataPath + "/Resources/HealthSystem.json", string.Empty);

        string path = Application.dataPath + "/Resources/HealthSystem.json";
        string json = JsonUtility.ToJson(this);
        System.IO.StreamWriter writer = new System.IO.StreamWriter(path, true);
        writer.WriteLine(json);
        writer.Close();
    }

    public Vector2 GetLastCheckpoint()
    {
        return lastCheckpoint;
    }

    public void SetLastCheckpoint(Vector3 checkpoint)
    {
        lastCheckpoint = checkpoint;
    }

    public void FallOffMap()
    {
        // The player loses a heart
        Damage();
        // The player is teleported to the last checkpoint
        player.transform.position = GetLastCheckpoint();
    }
}
