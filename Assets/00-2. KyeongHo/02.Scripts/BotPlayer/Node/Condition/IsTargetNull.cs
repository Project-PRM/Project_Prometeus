using UnityEngine;

public class IsTargetNull : IConditionNode
{
    private GameObject _target;
    
    public IsTargetNull(GameObject target) => _target = target;
    public ENodeState Evaluate()
    {
        return _target == null ? ENodeState.Success : ENodeState.Failure;
    }
}
