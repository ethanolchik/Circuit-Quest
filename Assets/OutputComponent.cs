public class OutputComponent : LogicGateComponent
{
    public OutputComponent() : base("OUTPUT") { }

    public override int ComputeOutput(int input1, int input2)
    {
        return input1;
    }
}