using UnityEngine;

public class AttackAction : IActionNode
{
    private EnemyController _enemy;

    public AttackAction(EnemyController enemy) => _enemy = enemy;

    public ENodeState Evaluate()
    {
        AttackPlayer();
        return ENodeState.Success;
    }

    public void AttackPlayer()
    {
        Debug.Log("공격!");
        // 애니메이션 실행 + 데미지
    }
}