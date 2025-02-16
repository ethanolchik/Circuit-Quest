using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInfo : MonoBehaviour
{
    public static GameInfo Instance { get; private set;}
    private List<string> previousScenes = new List<string>();

    private bool resetPos = false;

    public bool puzzleSolved = false;

    /// <summary>
    /// Keeps track of the unlocked levels
    /// 0: tutorial
    /// 1: level 1
    /// ...
    /// 5: level 5
    /// </summary>
    [SerializeField] private int levelUnlocked = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Load game data from the JSON file
            JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(Application.dataPath + "/Resources/GameInfo.json"), this);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NextLevel()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Tutorial level":
                if (levelUnlocked < 1)
                {
                    levelUnlocked = 1;
                }
                break;
            case "Level 1":
                if (levelUnlocked < 2)
                {
                    levelUnlocked = 2;
                }
                break;
            case "Level 2":
                if (levelUnlocked < 3)
                {
                    levelUnlocked = 3;
                }
                break;
            case "Level 3":
                if (levelUnlocked < 4)
                {
                    levelUnlocked = 4;
                }
                break;
            case "Level 4":
                if (levelUnlocked < 5)
                {
                    levelUnlocked = 5;
                }
                break;
            case "Level 5":
                break;
        }
    }

    public int CurrentLevel()
    {
        return levelUnlocked;
    }

    public void AddCurrentScene()
    {
        previousScenes.Add(SceneManager.GetActiveScene().path);
    }

    public string GetPreviousScene()
    {
        if (previousScenes.Count > 0)
        {
            string scene = previousScenes[previousScenes.Count - 1];
            previousScenes.RemoveAt(previousScenes.Count - 1);
            return scene;
        }
        return SceneManager.GetActiveScene().path;
    }

    public void SetResetPos(bool x)
    {
        resetPos = x;
    }

    public bool GetResetPos()
    {
        return resetPos;
    }

    public bool GetPuzzleSolved()
    {
        return puzzleSolved;
    }

    public void SetSolvePuzzle(bool b)
    {
        puzzleSolved = b;
    }

    void OnDestroy()
    {
        System.IO.File.WriteAllText(Application.dataPath + "/Resources/GameInfo.json", JsonUtility.ToJson(this));
    }
}
