using System.Collections.Generic;

public class Sequence : IBtNode
{
    private List<IBtNode> _children;

    public Sequence(List<IBtNode> children) => _children = children;

    public ENodeState Evaluate()
    {
        foreach (var child in _children)
        {
            var result = child.Evaluate();
            if (result != ENodeState.Success)
                return result;
        }
        return ENodeState.Success;
    }
}