using UnityEngine;

public class ChaseAction : IActionNode
{
    private EnemyController _enemy;
    private Transform _cachedTarget;

    public ChaseAction(EnemyController enemy) => _enemy = enemy;

    public ENodeState Evaluate()
    {
        ChasePlayer();
        return ENodeState.Running;
    }

    public void ChasePlayer()
    {
        if (_enemy.Target == null || _cachedTarget == _enemy.Target)
            return;
        _enemy.NavMeshAgent.isStopped = false;
        Debug.Log($"Chasing player: {_enemy.Target.name}");
        _enemy.NavMeshAgent.SetDestination(_enemy.Target.position);
        //_enemy.Animator.SetBool("isRunning", true);
    }
}