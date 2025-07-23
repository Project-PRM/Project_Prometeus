public class PatrolAction : BtNode
{
    private AIController _ai;

    public PatrolAction(AIController ai) => _ai = ai;

    public override ENodeState Evaluate()
    {
        _ai.Patrol();
        return ENodeState.Running;
    }
}
