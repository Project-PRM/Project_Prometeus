public class AttackAction : BtNode
{
    private AIController _ai;

    public AttackAction(AIController ai) => _ai = ai;

    public override ENodeState Evaluate()
    {
        _ai.AttackPlayer();
        return ENodeState.Success;
    }
}