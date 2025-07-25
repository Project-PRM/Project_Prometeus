using UnityEngine;

public class RushAttack : IActionNode
{
    private EnemyController _enemy;
    private float _chargeTime = 1.0f;
    private float _rushDuration = 1.5f;

    private float _timer = 0f;
    private float _lastAttackTime = 0f;
    private Vector3 _rushTargetPosition;

    private enum RushState { Idle, Charging, Rushing }
    private RushState _state = RushState.Idle;

    public RushAttack(EnemyController enemy)
    {
        _enemy = enemy;
    }

    public ENodeState Evaluate()
    {
        if (Time.time < _lastAttackTime + _enemy.EnemyData.AttackCoolTime)
        {
            return ENodeState.Failure;
        }

        switch (_state)
        {
            case RushState.Idle:
                _enemy.SelectTarget();
                if (_enemy.Target == null)
                    return ENodeState.Failure;

                SetRushTargetPosition();
                _enemy.NavMeshAgent.isStopped = true;
                _timer = 0f;
                _state = RushState.Charging;
                return ENodeState.Running;

            case RushState.Charging:
                _timer += Time.deltaTime;
                if (_timer >= _chargeTime)
                {
                    _enemy.NavMeshAgent.isStopped = false;
                    _enemy.NavMeshAgent.SetDestination(_rushTargetPosition);
                    _timer = 0f;
                    _state = RushState.Rushing;
                }
                return ENodeState.Running;

            case RushState.Rushing:
                _timer += Time.deltaTime;

                if (_timer >= _rushDuration || ReachedDestination())
                {
                    _enemy.NavMeshAgent.isStopped = true;
                    _lastAttackTime = Time.time;
                    _state = RushState.Idle;
                    return ENodeState.Success;
                }

                return ENodeState.Running;
        }

        return ENodeState.Failure;
    }

    private void SetRushTargetPosition()
    {
        var direction = (_enemy.Target.position - _enemy.transform.position).normalized;
        _rushTargetPosition = _enemy.Target.position + direction * 5f;
    }

    private bool ReachedDestination()
    {
        return Vector3.Distance(_enemy.transform.position, _rushTargetPosition) < 0.5f;
    }
}
