public class ORGate : LogicGateComponent
{
    public ORGate() : base("OR") { }

    public override int ComputeOutput(int input1, int input2)
    {
        return input1 | input2;  // OR logic
    }
}