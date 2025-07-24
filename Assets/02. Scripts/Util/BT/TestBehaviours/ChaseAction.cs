public class ChaseAction : BtNode
{
    private AIController _ai;

    public ChaseAction(AIController ai) => _ai = ai;

    public override ENodeState Evaluate()
    {
        _ai.ChasePlayer();
        return ENodeState.Running;
    }
}