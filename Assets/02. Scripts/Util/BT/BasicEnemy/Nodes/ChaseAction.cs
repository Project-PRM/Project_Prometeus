using UnityEngine;

public class ChaseAction : IActionNode
{
    private EnemyController _enemy;

    public ChaseAction(EnemyController enemy) => _enemy = enemy;

    public ENodeState Evaluate()
    {
        Debug.Log("Chase 실행중");

        if (_enemy.Target == null || !_enemy.IsPlayerDetected(_enemy.EnemyData.VisionRange))
        {
            Debug.Log("타겟 없음! 추적 실패");
            _enemy.NavMeshAgent.isStopped = true;
            _enemy.Animator.SetBool("isRunning", false);
            return ENodeState.Failure;
        }

        float distance = Vector3.Distance(_enemy.transform.position, _enemy.Target.position);
        if (distance <= _enemy.EnemyData.AttackRange)
        {
            Debug.Log("공격 범위 진입! 추적 성공");
            _enemy.NavMeshAgent.isStopped = true;
            _enemy.Animator.SetBool("isRunning", false);
            return ENodeState.Success;
        }

        ChasePlayer();
        return ENodeState.Running;
    }

    private void ChasePlayer()
    {
        _enemy.ResetAnimatorParameters();

        _enemy.NavMeshAgent.isStopped = false;
        _enemy.NavMeshAgent.speed = _enemy.EnemyData.SprintSpeed;

        _enemy.NavMeshAgent.SetDestination(_enemy.Target.position);
        _enemy.Animator.SetBool("isRunning", true);
    }
}
