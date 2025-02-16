using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GateEditor : MonoBehaviour
{
    /// <summary>
    /// Static instance of the GateEditor accessible by all logic gate components.
    /// </summary>
    public static GateEditor Instance;

    /// <summary>
    /// Assigned when an input button is pressed on a logic gate.
    /// </summary>
    private LogicGateComponent currentInputComponent;
    /// <summary>
    /// Assigned when an output button is pressed in a logic gate.
    /// </summary>
    private LogicGateComponent currentOutputComponent;

    /// <summary>
    /// Tracks whether input 1 or input 2 is being connected
    /// </summary>
    private int currentInputIndex;

    /// <summary>
    /// Prefab used to instantiate visual wire connections between logic gates.
    /// </summary>
    public GameObject wirePrefab;

    /// <summary>
    /// A list of all the wires connected in the current logic gate system.
    /// </summary>
    public List<Wire> wires = new List<Wire>();

    /// <summary>
    /// References to *all* the input gates discovered so far.
    /// </summary>
    public List<LogicGateComponent> inputGates;

    /// <summary>
    /// Reference to the final output gate (if any).
    /// </summary>
    public LogicGateComponent outputGate;

    /// <summary>
    /// Predefined / correct solution references.
    /// </summary>
    public List<LogicGateComponent> predefinedInputGates;
    public LogicGateComponent predefinedOutputGate;

    [SerializeField] private Button submitButton;

    public Texture inputOn;
    public Texture inputOff;

    public Texture outputOn;
    public Texture outputOff;

    public bool canProcede = false;

    [SerializeField] private bool isPuzzleScene;

    /// <summary>
    /// The number of inputs that the solution accepts.
    /// </summary>
    [SerializeField] private int validInputs = 1;

    /// <summary>
    /// A boolean value which states whether or not to use validInputs
    /// </summary>
    [SerializeField] private bool useValidInputs = false;

    void Awake()
    {
        Instance = this;
        
        var x = new TraceTable();

        x.Test();
    }

    void Start()
    {
        if (submitButton != null)
            submitButton.onClick.AddListener(Submit);
    }

    /// <summary>
    /// Method to set the input connection when the player clicks on an input button
    /// </summary>
    /// <param name="gate">The gate which the input should be set to</param>
    /// <param name="inputIndex">The index of the gate's input that is connected</param>
    public void SetInputConnection(LogicGateComponent gate, int inputIndex)
    {
        currentInputComponent = gate;
        currentInputIndex = inputIndex;

        if (currentOutputComponent != null)
        {
            TryConnectWires();
        }
    }

    /// <summary>
    /// Method to set the output connection when the player clicks on an output button
    /// </summary>
    /// <param name="gate">The gate which the output should be set to</param>
    public void SetOutputConnection(LogicGateComponent gate)
    {
        currentOutputComponent = gate;
        if (currentInputComponent != null)
        {
            TryConnectWires();
        }
    }

    /// <summary>
    /// Attempt to connect the wire if both input and output have been selected
    /// </summary>
    void TryConnectWires()
    {
        // Check if both input and output have been selected
        if (currentInputComponent != null && currentOutputComponent != null)
        {
            // Prevent connecting a gate to itself
            if (currentInputComponent.gameObject == currentOutputComponent.gameObject)
            {
                currentInputComponent = null;
                currentOutputComponent = null;
                return;
            }

            // Remove existing input wire at this index if it exists
            Wire existingWire = currentInputComponent.GetInputWire(currentInputIndex);
            if (existingWire != null)
            {
                currentInputComponent.RemoveInputWire(currentInputIndex);
            }

            // Remove existing output wire if it exists
            Wire existingOutputWire = currentOutputComponent.GetOutputWire();
            if (existingOutputWire != null)
            {
                currentOutputComponent.RemoveOutputWire();
            }

            // Create the new wire
            GameObject newWire = Instantiate(wirePrefab);
            Wire wire = newWire.GetComponent<Wire>();
            wire.SetInput(currentInputComponent, currentInputIndex);
            wire.SetOutput(currentOutputComponent);

            currentInputComponent.SetInputWire(currentInputIndex, wire);
            currentOutputComponent.SetOutputWire(wire);

            // Actually set the logic input
            currentInputComponent.SetInput(currentOutputComponent, currentInputIndex);

            wire.SetShouldUpdate(true);
            wires.Add(wire);

            // Instead of adding to inputGates in random order,
            // we just check if either side is an INPUT gate and track it.
            if (currentInputComponent.GetName() == "INPUT" && !inputGates.Contains(currentInputComponent))
            {
                inputGates.Add(currentInputComponent);
            }
            if (currentOutputComponent.GetName() == "INPUT" && !inputGates.Contains(currentOutputComponent))
            {
                inputGates.Add(currentOutputComponent);
            }

            // If either component is "OUTPUT", set it as outputGate
            if (currentInputComponent.GetName() == "OUTPUT")
            {
                outputGate = currentInputComponent;
            }
            if (currentOutputComponent.GetName() == "OUTPUT")
            {
                outputGate = currentOutputComponent;
            }

            currentInputComponent = null;
            currentOutputComponent = null;
        }
    }

    /// <summary>
    /// Called when the player's solution is submitted for assessment.
    /// </summary>
    public void Submit()
    {
        // Update the final state of each wire's logic before validating
        foreach (var wire in wires)
        {
            wire.GetOutput().SetState(
                wire.GetInput().ComputeOutput(
                    wire.GetInput().GetState(), 
                    wire.GetOutput().GetState()
                )
            );
        }

        Validate();
    }

    /// <summary>
    /// Used to validate the player's submission using the TraceTable class.
    /// </summary>
    public void Validate()
    {
        // Check if the player has exceeded the number of valid inputs, if useValidInputs is true.
        if ((inputGates.Count != validInputs) && useValidInputs)
        {
            SubmissionMessage.Instance.ShowIncorrect();
            return;
        }

        // Sort inputGates by something stable (instanceID).
        // This ensures the same ordering for the trace table
        // no matter how wires were connected in the editor.
        SortInputGatesByInstanceID();

        // Build the player's table
        TraceTable playerTable = new TraceTable();
        playerTable.Populate(inputGates, outputGate);

        // Build the solution table
        TraceTable solutionTable = new TraceTable();
        solutionTable.Populate(predefinedInputGates, predefinedOutputGate);

        // Compare
        if (playerTable.Equals(solutionTable))
        {
            SubmissionMessage.Instance.ShowCorrect();
        }
        else
        {
            SubmissionMessage.Instance.ShowIncorrect();
        }
    }

    /// <summary>
    /// Sorts the inputGates list by each gate's instanceID 
    /// so the order is stable every time we do a trace table.
    /// </summary>
    private void SortInputGatesByInstanceID()
    {
        inputGates.Sort((a, b) => a.GetInstanceID().CompareTo(b.GetInstanceID()));
    }

    /// <summary>
    /// Removes a wire from the system
    /// </summary>
    public void RemoveWire(Wire wire)
    {
        wires.Remove(wire);
        Destroy(wire.gameObject);
    }

    /// <summary>
    /// Post correct solution (Logic to be executed after the player has solved the puzzle)
    /// </summary>
    public void PostCorrect()
    {
        GameInfo.Instance.SetSolvePuzzle(true);
        SceneManager.LoadSceneAsync(GameInfo.Instance.GetPreviousScene());
    }

    /// <summary>
    /// Post incorrect solution (Logic to be executed after the player has failed the puzzle)
    /// </summary>
    public void PostIncorrect()
    {
        if (isPuzzleScene)
        {
            GameInfo.Instance.SetResetPos(true);
            SceneManager.LoadSceneAsync(GameInfo.Instance.GetPreviousScene());
        }
    }
}
