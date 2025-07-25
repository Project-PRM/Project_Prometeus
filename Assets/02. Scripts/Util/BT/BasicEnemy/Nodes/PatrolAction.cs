using UnityEngine;

public class PatrolAction : IActionNode
{
    private EnemyController _enemy;
    private Transform _nextPatrolPoint;

    public PatrolAction(EnemyController enemy) => _enemy = enemy;

    public ENodeState Evaluate()
    {
        Debug.Log("Patrol 실행중");

        if (_enemy.Target != null || _enemy.IsPlayerDetected(_enemy.EnemyData.VisionRange))
        {
            Debug.Log("플레이어 감지! 패트롤 실패");
            _enemy.NavMeshAgent.isStopped = true;
            _enemy.Animator.SetBool("isWalking", false);
            _nextPatrolPoint = null;
            return ENodeState.Failure;
        }

        if (_nextPatrolPoint == null)
        {
            ChooseNextPatrolPoint();
        }
        else if (Vector3.Distance(_enemy.transform.position, _nextPatrolPoint.position) < 1f)
        {
            Debug.Log($"패트롤 지점 {_nextPatrolPoint.name} 도착!");
            _enemy.NavMeshAgent.isStopped = true;
            _enemy.Animator.SetBool("isWalking", false);
            _nextPatrolPoint = null;
            return ENodeState.Success;
        }

        Patrol();
        return ENodeState.Running;
    }

    private void ChooseNextPatrolPoint()
    {
        int idx = Random.Range(0, _enemy.PatrolPoints.Length);
        _nextPatrolPoint = _enemy.PatrolPoints[idx];
        Debug.Log($"새로운 패트롤 지점: {_nextPatrolPoint.name}");
    }

    private void Patrol()
    {
        _enemy.ResetAnimatorParameters();
        _enemy.NavMeshAgent.isStopped = false;
        _enemy.NavMeshAgent.speed = _enemy.EnemyData.Speed;
        _enemy.NavMeshAgent.SetDestination(_nextPatrolPoint.position);
        _enemy.Animator.SetBool("isWalking", true);
    }
}
