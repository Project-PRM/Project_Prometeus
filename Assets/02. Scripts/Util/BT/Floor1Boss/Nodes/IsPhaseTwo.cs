using UnityEngine;

public class IsPhaseTwo : IConditionNode
{
    private EnemyController _enemy;

    public IsPhaseTwo(EnemyController enemy) => _enemy = enemy;

    public ENodeState Evaluate()
    {
        if (_enemy.CurrentHealth >= _enemy.EnemyData.MaxHealth * 0.3f)
        {
            return ENodeState.Success;
        }
        return ENodeState.Failure;
    }
}
