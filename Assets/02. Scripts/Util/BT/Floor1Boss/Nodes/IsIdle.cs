using UnityEngine;

public class IsIdle : IConditionNode
{
    private EnemyController _enemy;
    public IsIdle(EnemyController enemy) => _enemy = enemy;

    public ENodeState Evaluate()
    {
        return IsIdleState() ? ENodeState.Success : ENodeState.Failure;
    }

    private bool IsIdleState()
    {
        Collider[] hits = Physics.OverlapSphere(_enemy.transform.position, _enemy.EnemyData.VisionRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }
}