public class IsPlayerInAttackRange : BtNode
{
    private AIController _ai;

    public IsPlayerInAttackRange(AIController ai) => _ai = ai;

    public override ENodeState Evaluate()
    {
        return _ai.IsPlayerInAttackRange() ? ENodeState.Success : ENodeState.Failure;
    }
}