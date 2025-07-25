using System.Collections.Generic;

public class Sequence : IBtNode
{
    private List<IBtNode> _children;
    private int _runningChildIndex = 0;

    public Sequence(List<IBtNode> children) => _children = children;

    public ENodeState Evaluate()
    {
        for (int i = _runningChildIndex; i < _children.Count; i++)
        {
            var result = _children[i].Evaluate();

            if (result == ENodeState.Failure)
            {
                // 실패했으니 running 상태 초기화
                _runningChildIndex = 0;
                return ENodeState.Failure;
            }
            else if (result == ENodeState.Running)
            {
                // running 중인 자식 인덱스 저장 후 반환
                _runningChildIndex = i;
                return ENodeState.Running;
            }
            // Success면 다음 자식으로 진행
        }

        // 모두 성공했으니 running 인덱스 초기화 후 Success 반환
        _runningChildIndex = 0;
        return ENodeState.Success;
    }
}
