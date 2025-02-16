using UnityEngine;
using System.Collections.Generic;
using System.Linq;


[CreateAssetMenu(fileName = "GateLibrary", menuName = "Circuits/GateLibrary")]
public class GateLibrary : ScriptableObject
{
    /// <summary>
    /// List of gate prefabs for each gate type
    /// </summary>
    [SerializeField] private List<GatePrefabConfig> gatePrefabs;

    public GameObject GetPrefab(string gateType)
    {
        // Find the prefab for the given gate type
        var config = gatePrefabs.FirstOrDefault(x => x.gateTypeName == gateType);
        if (config != null && config.prefab != null)
        {
            return config.prefab;
        }
        else
        {
            Debug.LogError($"No prefab found for gate type: {gateType}");
            return null;
        }
    }
}

[System.Serializable]
public class GatePrefabConfig
{
    public string gateTypeName;    // e.g. "AND", "OR", "NOT", "XOR"
    public GameObject prefab;      // prefab that implements LogicGateComponent
}
