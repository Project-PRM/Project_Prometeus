using UnityEngine;

public class IsPhaseOne : IConditionNode
{
    private EnemyController _enemy;

    public IsPhaseOne(EnemyController enemy) => _enemy = enemy;

    public ENodeState Evaluate()
    {
        if (_enemy.CurrentHealth >= _enemy.EnemyData.MaxHealth * 0.6f)
        {
            return ENodeState.Success;
        }
        return ENodeState.Failure;
    }
}
