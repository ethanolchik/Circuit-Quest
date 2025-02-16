using UnityEngine;
using System.Collections.Generic;

public class CircuitInitialiser : MonoBehaviour
{
    /// <summary>
    /// The blueprint of the circuit to be loaded
    /// </summary>
    [SerializeField] private CircuitBlueprint blueprint;

    /// <summary>
    /// A ScriptableObject containing all the gate prefabs
    /// </summary>
    [SerializeField] private GateLibrary gateLibrary;

    /// <summary>
    /// The parent object under which all gates will be instantiated
    /// </summary>
    [SerializeField] private Transform editor;

    // A dictionary to quickly lookup gate instances by their ID
    private Dictionary<string, LogicGateComponent> gateDict = new Dictionary<string, LogicGateComponent>();

    void Start()
    {
        // Instantiate all gates
        foreach (var gateDef in blueprint.gates)
        {
            // Use the GateLibrary class to get the right prefab
            GameObject gatePrefab = gateLibrary.GetPrefab(gateDef.gateType);
            if (gatePrefab == null)
            {
                // If the gate doesn't have a prefab, skip it (should not happen)
                Debug.LogWarning($"Skipping gateID '{gateDef.gateID}' due to missing prefab for '{gateDef.gateType}'");
                continue;
            }

            // Instantiate the prefab as a child of this object
            GameObject gateInstance = Instantiate(gatePrefab, this.transform);
            gateInstance.name = gateDef.gateID; // So we can see it in the Hierarchy
            gateInstance.SetActive(true); // Make sure it's active
            gateInstance.transform.SetParent(editor); // Set the parent to the editor canvas
            gateInstance.transform.localScale = Vector3.one; // Reset scale
            gateInstance.transform.localPosition = new Vector3(0, -3000, 0); // Set the position so its not visible on the screen

            // Get the LogicGateComponent on the prefab
            LogicGateComponent gateComp = gateInstance.GetComponent<LogicGateComponent>();
            if (gateComp == null)
            {
                // This should never happen if the prefabs are set up correctly
                Debug.LogError($"Prefab for '{gateDef.gateType}' does not have a LogicGateComponent. Check your setup.");
                continue;
            }

            // Add to dictionary for later connections
            gateDict[gateDef.gateID] = gateComp;

            // initialise the value of GateEditor.predefinedInputGates and GateEditor.predefinedOutputGate
            if (gateDef.gateType == "INPUT")
            {
                GateEditor.Instance.predefinedInputGates.Add(gateComp);
            }
            else if (gateDef.gateType == "OUTPUT")
            {
                GateEditor.Instance.predefinedOutputGate = gateComp;
            }
        }

        // Make all connections
        foreach (var connDef in blueprint.connections)
        {
            // Get the source and target gates from the dictionary
            // e.g. "G1" -> "G2" at inputIndex
            if (!gateDict.ContainsKey(connDef.sourceGateID))
            {
                Debug.LogError($"Source gate '{connDef.sourceGateID}' not found in dictionary.");
                continue;
            }
            if (!gateDict.ContainsKey(connDef.targetGateID))
            {
                Debug.LogError($"Target gate '{connDef.targetGateID}' not found in dictionary.");
                continue;
            }

            // Get the source and target gates
            LogicGateComponent sourceGate = gateDict[connDef.sourceGateID];
            LogicGateComponent targetGate = gateDict[connDef.targetGateID];

            // Connect the gates
            targetGate.SetInput(sourceGate, connDef.targetInputIndex);
        }

        Debug.Log("Circuit initialised from blueprint.");
    }
}
