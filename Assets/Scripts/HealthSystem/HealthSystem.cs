using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class HealthSystem : MonoBehaviour
{
    public static HealthSystem Instance;
    [SerializeField] private int hearts;

    /// <summary>
    /// 3 full hearts
    /// </summary>
    private Sprite heart3;
    /// <summary>
    /// 2 full hearts, 1 empty heart
    /// </summary>
    private Sprite heart2;
    /// <summary>
    /// 1 full heart, 2 empty hearts
    /// </summary>
    private Sprite heart1;
    /// <summary>
    /// 3 empty hearts
    /// </summary>
    private Sprite heart0;

    /// <summary>
    /// The area where the hearts will be shown on the screen
    /// </summary>
    private SpriteRenderer healthbar;
    [SerializeField] private GameObject player;
    [SerializeField] private Vector2 lastCheckpoint = new Vector2(0, 1);
    [SerializeField] private Vector2 bridgeCheckpoint = new Vector2(0, 1);

    [SerializeField] private bool bridgePuzzle = false;

    public bool shouldShowPuzzle = true;

    [SerializeField] private bool healthbarUnlocked;

    void Awake()
    {
        Instance = this;
        heart3 = Resources.Load<Sprite>("hearts3");
        heart2 = Resources.Load<Sprite>("hearts2");
        heart1 = Resources.Load<Sprite>("hearts1");
        heart0 = Resources.Load<Sprite>("hearts0");
    }

    void Start()
    {
        // Check if the JSON file formatting is incorrect
        try
        {
            JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(Application.dataPath + "/Resources/HealthSystem.json"), this);
        } catch {
            // Reset the variables to their default settings
            hearts = 3;
        }

        player = GameObject.Find("Player");
        healthbar = player.transform.Find("healthbar").GetComponent<SpriteRenderer>();
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

                GameOver();
                break;
        }

        if (healthbarUnlocked && SceneManager.GetActiveScene().name == "Tutorial level")
        {
            healthbar.enabled = true;
        }

        if (GameInfo.Instance.GetResetPos())
        {
            GameInfo.Instance.SetResetPos(false);
            // Reset the player's position and reset the last checkpoint attribute
            player.transform.position = new Vector2(0, 1);
            SetLastCheckpoint(new Vector2(0, 1));
        }

        if (bridgePuzzle)
        {
            player.transform.position = bridgeCheckpoint;
            player.transform.position = new Vector2(player.transform.position.x-5, player.transform.position.y);
            bridgePuzzle = false;
        }
    }

    void Update()
    {
        UpdateHealth();
    }

    void UpdateHealth()
    {
        if (hearts < 0 || hearts > 3) {
            hearts = 3;
        }

        switch (hearts)
        {
            case 3:
                healthbar.sprite = heart3;
                break;
            case 2:
                healthbar.sprite = heart2;
                break;
            case 1:
                healthbar.sprite = heart1;
                break;
            case 0:
                healthbar.sprite = heart0;
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
                return;
        }

        player.GetComponent<PlayerMovement>().Dim();
        StartCoroutine(Wait());
    }

    private void GameOver()
    {
        // Reset the player's health
        hearts = 3;
        healthbar.sprite = heart3;

        // Skip the game-over puzzles
        if (shouldShowPuzzle)
        {
            player.transform.position = GetLastCheckpoint();
            return;
        }

        int random = Random.Range(0, 4);

        // Randomly load a game-over puzzle scene
        GameInfo.Instance.AddCurrentScene();

        switch (random)
        {
            case 0:
                SceneManager.LoadSceneAsync("Scenes/Logic Gate Puzzles/Game Over Puzzle 1");
                break;
            case 1:
                SceneManager.LoadSceneAsync("Scenes/Logic Gate Puzzles/Game Over Puzzle 2");
                break;
            case 2:
                SceneManager.LoadSceneAsync("Scenes/Logic Gate Puzzles/Game Over Puzzle 3");
                break;
            case 3:
                SceneManager.LoadSceneAsync("Scenes/Logic Gate Puzzles/Game Over Puzzle 4");
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

    public Vector2 GetBridgeCheckpoint()
    {
        return bridgeCheckpoint;
    }

    public void SetBridgeCheckpoint(Vector3 checkpoint)
    {
        bridgeCheckpoint = checkpoint;
    }

    public bool GetBridgePuzzle()
    {
        return bridgePuzzle;
    }

    public void SetBridgePuzzle(bool value)
    {
        bridgePuzzle = value;
    }

    public void UnlockHealthbar()
    {
        healthbarUnlocked = true;
    }

    public void LockHealthbar()
    {
        healthbarUnlocked = false;
    }

    public void FallOffMap()
    {
        // The player loses a heart
        Damage();
        // The player is teleported to the last checkpoint
        player.transform.position = GetLastCheckpoint();
    }
}
