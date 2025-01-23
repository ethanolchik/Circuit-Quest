using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CircuitBlueprint", menuName = "Circuits/Blueprint")]
public class CircuitBlueprint : ScriptableObject
{
    // List of all gates in this circuit
    public List<GateDefinition> gates;

    // List of connections describing how the gates are wired
    public List<ConnectionDefinition> connections;
}

[System.Serializable]
public class GateDefinition
{
    public string gateID;   // e.g. "G1", "G2", "InputA", etc.
    public string gateType; // e.g. "AND", "OR", "NOT", "XOR", "INPUT", "OUTPUT"
}

[System.Serializable]
public class ConnectionDefinition
{
    public string sourceGateID; // e.g. "G1"
    public string targetGateID; // e.g. "G2"
    public int targetInputIndex; // the input index on the target gate to connect to
}
