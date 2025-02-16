public class NotGate : LogicGateComponent
{
    public NotGate() : base("NOT") {}

    public override int ComputeOutput(int input1, int input2)
    {
        return ~input1 & 1;
    }
}