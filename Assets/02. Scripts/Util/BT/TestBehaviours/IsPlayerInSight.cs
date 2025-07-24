public class IsPlayerInSight : BtNode
{
    private AIController _ai;

    public IsPlayerInSight(AIController ai) => _ai = ai;

    public override ENodeState Evaluate()
    {
        return _ai.IsPlayerInSight() ? ENodeState.Success : ENodeState.Failure;
    }
}