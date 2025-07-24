using UnityEngine;

public class IsPlayerInSight : IConditionNode
{
    private EnemyController _enemy;

    public IsPlayerInSight(EnemyController enemy) => _enemy = enemy;

    public ENodeState Evaluate()
    {
        return CheckIsPlayerInSight() ? ENodeState.Success : ENodeState.Failure;
    }

    public Transform FindNearestPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(_enemy.transform.position, _enemy.EnemyData.VisionRange);
        float minDist = float.MaxValue;
        Transform nearest = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                float dist = Vector3.Distance(_enemy.transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = hit.transform;
                }
            }
        }

        return nearest;
    }

    public bool CheckIsPlayerInSight()
    {
        _enemy.SetTarget(FindNearestPlayer());
        return _enemy.Target != null;
    }
}