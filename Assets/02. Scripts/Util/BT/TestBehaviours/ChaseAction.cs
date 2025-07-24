using UnityEngine;

public class ChaseAction : IActionNode
{
    private EnemyController _enemy;

    public ChaseAction(EnemyController enemy) => _enemy = enemy;

    public ENodeState Evaluate()
    {
        ChasePlayer();
        return ENodeState.Running;
    }

    public void ChasePlayer()
    {
        Debug.Log("추격 중");
        // 플레이어 쪽으로 이동
    }
}