using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BridgeManager : MonoBehaviour
{
    public static BridgeManager Instance;

    /// <summary>
    /// The bridges in the current scene
    /// </summary>
    [SerializeField] private List<GameObject> bridges;

    /// <summary>
    /// An index up to which all of the bridges have been solved.
    /// </summary>
    [SerializeField] private int solvedIndex;

    private bool shouldIncrement = false;

    void Awake()
    {
        Instance = this;
        JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(Application.dataPath + "/Resources/BridgeManager.json"), this);
        shouldIncrement = GameInfo.Instance.GetPuzzleSolved()
            && HealthSystem.Instance.GetBridgePuzzle();
        if (shouldIncrement)
        {
            solvedIndex++;
        }
    }

    void Start()
    {
        FindBridges();
        SetSolvedBridges();
    }

    void FindBridges()
    {
        var b = GameObject.FindGameObjectsWithTag("Bridge").OrderByDescending(x => x.transform.position.x).Reverse();

        bridges = new List<GameObject>(b);
    }

    void SetSolvedBridges()
    {
        // Only start spawning bridges in their solved position after the second puzzle has been solved.
        if (solvedIndex > 1)
        {
            // Iterate between bridges[0] to bridge[n-1] and set their positions to their solved position
            for (int i = 0; i < solvedIndex-1 && i < bridges.Count; i++)
            {
                Transform solvedChild = bridges[i].transform.Find("Solved");
                bridges[i].transform.position = solvedChild.position;

                bridges[i].BroadcastMessage("DestroyProximityMessage");
            }
        }

        // Make sure that it only starts moving the bridge if the bridge puzzle is solved
        if (shouldIncrement)
            // Solve bridges[n]
            bridges[solvedIndex-1].BroadcastMessage("InvokeMoveBridge");
    }

    public void ResetIndex()
    {
        solvedIndex = 0;
    }

    void OnDestroy()
    {
        // Save the bridge manager state to a JSON file
        System.IO.File.WriteAllText(Application.dataPath + "/Resources/BridgeManager.json", JsonUtility.ToJson(this));
    }
}
