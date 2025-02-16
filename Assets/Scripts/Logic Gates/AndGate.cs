public class ANDGate : LogicGateComponent
{
    public ANDGate() : base("AND") { }

    public override int ComputeOutput(int input1, int input2)
    {
        return input1 & input2;  // AND logic
    }
}