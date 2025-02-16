using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogicGateComponent : MonoBehaviour
{
    /// <summary>
    /// The current state of the logic gate (either 1 or 0)
    /// </summary>
    public int state = 0;

    public LogicGateComponent[] inputs;
    /// <summary>
    /// The name of the logic gate e.g. AND, NOT etc.
    /// </summary>
    private string gateName;

    /// <summary>
    /// Stores wires connected to each input
    /// </summary>
    private Wire[] inputWires;

    /// <summary>
    /// Stores the wire connected to the output
    /// </summary>
    private Wire outputWire;

    public bool shouldUpdateState = false;

    // Button references for manual wire connections
    [SerializeField] private Button input1Button;
    [SerializeField] private Button input2Button;
    [SerializeField] private Button outputButton;
    [SerializeField] private Button _switch;

    [SerializeField] private GameObject gateIcon;

    void Start() {
        if (input1Button != null)
            input1Button.onClick.AddListener(OnInput1ButtonClick);
        if (input2Button != null)
            input2Button.onClick.AddListener(OnInput2ButtonClick);
        if (outputButton != null)
            outputButton.onClick.AddListener(OnOutputButtonClick);
        if (_switch != null)
            _switch.onClick.AddListener(OnSwitchClick);
    }

    void Update()
    {
        PropagateState();

        // Short-circuit evaluation
        if (shouldUpdateState)
        {
            SetState(state);
            PropagateState();
        }
        else
        {
            if (gateName == "INPUT")
            {
                SetState(ComputeOutput(state, 0));
                if (state == 0)
                {
                    gateIcon.GetComponent<RawImage>().texture = GateEditor.Instance.inputOff;
                } else {
                    gateIcon.GetComponent<RawImage>().texture = GateEditor.Instance.inputOn;
                }
            }
            else if (gateName == "OUTPUT")
            {
                // Check if the input connections exist and get their states
                if (inputs.Length > 0 && inputs[0] != null)
                {
                    SetState(inputs[0].GetState());

                    if (state == 0)
                    {
                        gateIcon.GetComponent<RawImage>().texture = GateEditor.Instance.outputOff;
                    } else {
                        gateIcon.GetComponent<RawImage>().texture = GateEditor.Instance.outputOn;
                    }
                } else {
                    if (state == 1) {
                        state = 0;
                        gateIcon.GetComponent<RawImage>().texture = GateEditor.Instance.outputOff;
                    }
                }

                if (gameObject.name == "Play" && state == 1)
                {
                    GameInfo.Instance.AddCurrentScene();
                    SceneManager.LoadSceneAsync("Scenes/LevelSelect");
                } else if (gameObject.name == "Quit" && state == 1)
                {
                    Application.Quit();
                }
            }
            else
            {
                int input1 = 0;
                int input2 = 0;

                // Check if the input connections exist and get their states
                if (inputs.Length > 0 && inputs[0] != null)
                {
                    input1 = inputs[0].GetState();
                }

                if (inputs.Length > 1 && inputs[1] != null)
                {
                    input2 = inputs[1].GetState();
                }

                // Calculate the new state of the logic gate
                int newState = ComputeOutput(input1, input2);

                // Update the state of the logic gate if it has changed
                if (state != newState)
                {
                    SetState(newState);
                }
            }
        }
    }

    /// <summary>
    /// Used to update the state of the output wire whenever the state of the logic gate changes.
    /// </summary>
    protected void PropagateState()
    {
        Wire outputWire = GetOutputWire();

        // Update the state of the output wire if it exists
        if (outputWire != null)
        {
            outputWire.SetState(state);
        }
    }

    /// <summary>
    /// Creates a new logic gate with the given name of the logic gate, e.g. AND, OR etc.
    /// </summary>
    /// <param name="name">The name of the logic gate</param>
    public LogicGateComponent(string name)
    {
        this.gateName = name;
        inputs = new LogicGateComponent[2];  // Most gates have 2 inputs
        inputWires = new Wire[2]; // Initialize the input wires array
    }

    /// <summary>
    /// Calculates output based on inputs - to be overridden by specific gates
    /// </summary>
    /// <param name="input1"></param>
    /// <param name="input2"></param>
    /// <returns></returns>
    public virtual int ComputeOutput(int input1, int input2)
    {
        return 0;  // Base class doesn't have logic
    }

    /// <summary>
    /// Sets the input connections for this gate
    /// </summary>
    /// <param name="newInputs">The list of the new inputs to be assigned to this logic gate.</param>
    public void SetInput(LogicGateComponent newInput, int index)
    {
        inputs[index] = newInput;
    }

    /// <summary>
    /// Returns the array of input connections
    /// </summary>
    /// <returns></returns>
    public LogicGateComponent[] GetInputs() {
        return inputs;
    }

    /// <summary>
    /// Gets the current state (0 or 1) of this gate
    /// </summary>
    /// <returns></returns>
    public int GetState() => state;
    
    /// <summary>
    /// Sets the current state (0 or 1) of this gate
    /// </summary>
    /// <param name="newState">The value of the new state</param>
    public void SetState(int newState) {
        // Make sure that we are not setting the state to anything other than 0 or 1
        if (state == 0 || state == 1)
            state = newState;
    }

    /// <summary>
    /// Handles click on first input connection button
    /// </summary>
    public void OnInput1ButtonClick()
    {
        GateEditor.Instance.SetInputConnection(this, 0);  // Connect input 1
    }

    /// <summary>
    /// Handles click on second input connection button
    /// </summary>
    public void OnInput2ButtonClick()
    {
        GateEditor.Instance.SetInputConnection(this, 1);  // Connect input 2
    }

    /// <summary>
    /// Handles click on output connection button
    /// </summary>
    public void OnOutputButtonClick()
    {
        GateEditor.Instance.SetOutputConnection(this);  // Connect output
    }

    /// <summary>
    /// Handles click on a switch for an input button
    /// </summary>
    private void OnSwitchClick()
    {
        // When the switch is pressed, flip the state
        int newState = state == 0 ? 1 : 0;
        SetState(newState);
        PropagateState();
    }

    /// <summary>
    /// Gets the world position of the currently selected connection point
    /// </summary>
    /// <returns></returns>
    /// <param name="inputIndex">The index of the currently selected connection point</param>
    public Vector3 GetInputPosition(int inputIndex)
    {
        if (inputIndex == 0)
        {
            return input1Button.GetComponent<RectTransform>().position;
        }
        else if (inputIndex == 1)
        {
            return input2Button.GetComponent<RectTransform>().position;
        }
        else
        {
            return Vector3.zero; // Handle invalid index appropriately
        }
    }

    /// <summary>
    /// The position of the currently selected output
    /// </summary>
    /// <returns></returns>
    public Vector3 GetOutputPosition()
    {
        return outputButton.GetComponent<RectTransform>().position;
    }

    /// <summary>
    /// Method to set wire for a specific input
    /// </summary>
    public void SetInputWire(int inputIndex, Wire wire)
    {
        inputWires[inputIndex] = wire;
    }

    /// <summary>
    /// Method to get wire for a specific input
    /// </summary>
    public Wire GetInputWire(int inputIndex)
    {
        return inputWires[inputIndex];
    }

    /// <summary>
    /// Method to remove wire from a specific input
    /// </summary>
    public void RemoveInputWire(int inputIndex)
    {
        if (inputWires[inputIndex] != null)
        {
            Wire wire = inputWires[inputIndex];
            inputWires[inputIndex] = null;
            inputs[inputIndex] = null;

            GateEditor.Instance.RemoveWire(wire);
        }
    }

    /// <summary>
    /// Sets the wire connected to the output
    /// </summary>
    public void SetOutputWire(Wire wire)
    {
        outputWire = wire;
    }

    /// <summary>
    /// Gets the wire connected to the output
    /// </summary>
    public Wire GetOutputWire()
    {
        return outputWire;
    }

    /// <summary>
    /// Removes the wire connected to the output
    /// </summary>
    public void RemoveOutputWire()
    {
        if (outputWire != null)
        {
            Wire wire = outputWire;
            outputWire = null;

            GateEditor.Instance.RemoveWire(wire);
        }
    }

    public void DestroyWires()
    {
        // Remove input wires
        for (int i = 0; i < inputWires.Length; i++)
        {
            if (inputWires[i] != null)
            {
                GateEditor.Instance.RemoveWire(inputWires[i]);
            }
        }

        // Remove output wire
        if (outputWire != null)
        {
            GateEditor.Instance.RemoveWire(outputWire);
        }
    }

    public string GetName()
    {
        return gateName;
    }

    void OnDestroy()
    {
        if (GateEditor.Instance.inputGates.Contains(this))
        {
            GateEditor.Instance.inputGates.Remove(this);
        }
        else if (GateEditor.Instance.outputGate == this)
        {
            GateEditor.Instance.outputGate = null;
        }
    }
}
