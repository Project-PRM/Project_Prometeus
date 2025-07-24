using System.Collections.Generic;

public class Selector : IBtNode
{
    private List<IBtNode> _children;

    public Selector(List<IBtNode> children) => _children = children;

    public ENodeState Evaluate()
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