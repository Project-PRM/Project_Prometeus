using System.Collections.Generic;

public class Sequence : BtNode
{
    private List<BtNode> _children;

    public Sequence(List<BtNode> children) => _children = children;

    public override ENodeState Evaluate()
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