using UnityEngine.UI;
using UnityEngine;

public class InputComponent : LogicGateComponent
{
    public InputComponent() : base("INPUT") { }

    public override int ComputeOutput(int input1, int input2)
    {
        return input1;
    }
}