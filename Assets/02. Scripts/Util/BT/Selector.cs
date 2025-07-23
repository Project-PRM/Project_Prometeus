using System.Collections.Generic;

public class Selector : BtNode
{
    private List<BtNode> _children;

    public Selector(List<BtNode> children) => _children = children;

    public override ENodeState Evaluate()
    {
        foreach (var child in _children)
        {
            var result = child.Evaluate();
            if (result == ENodeState.Success || result == ENodeState.Running)
                return result;
        }
        return ENodeState.Failure;
    }
}