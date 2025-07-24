using UnityEngine;

public class IsPlayerInAttackRange : IConditionNode
{
    private EnemyController _enemy;

    public IsPlayerInAttackRange(EnemyController enemy) => _enemy = enemy;

    public ENodeState Evaluate()
    {
        return CheckIsPlayerInAttackRange() ? ENodeState.Success : ENodeState.Failure;
    }

    public bool CheckIsPlayerInAttackRange()
    {
        if (_enemy.Target == null) return false;
        return Vector3.Distance(_enemy.transform.position, _enemy.Target.position) < _enemy.EnemyData.AttackRange;
    }
}