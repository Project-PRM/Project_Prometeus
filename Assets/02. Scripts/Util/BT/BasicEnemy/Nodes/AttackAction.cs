using Photon.Pun;
using UnityEngine;

public class AttackAction : IActionNode
{
    private EnemyController _enemy;
    private float _lastAttackTime = Time.time;

    public AttackAction(EnemyController enemy) => _enemy = enemy;

    public ENodeState Evaluate()
    {
        Debug.Log("Attack 실행중");

        if (_enemy.Target == null)
        {
            return ENodeState.Failure;
        }

        if (Time.time < _lastAttackTime + _enemy.EnemyData.AttackCoolTime)
        {
            _enemy.NavMeshAgent.isStopped = true;
            _enemy.Animator.SetBool("isAttacking", false);
            return ENodeState.Running; // 쿨타임 중
        }

        float distance = Vector3.Distance(_enemy.transform.position, _enemy.Target.position);
        if (distance > _enemy.EnemyData.AttackRange)
        {
            _enemy.NavMeshAgent.isStopped = true;
            _enemy.Animator.SetBool("isAttacking", false);
            return ENodeState.Failure; // 범위 밖
        }

        AttackPlayer();
        return ENodeState.Success;
    }

    public void AttackPlayer()
    {
        if (_enemy.Target == null)
            return;

        _enemy.transform.LookAt(_enemy.Target.position);
        _enemy.ResetAnimatorParameters();
        _enemy.Animator.SetBool("isAttacking", true);

        _enemy.NavMeshAgent.isStopped = true;
        Debug.Log($"Attack {_enemy.Target}: {_enemy.EnemyData.Damage}");

        if (_enemy.Target.TryGetComponent<IDamageable>(out var damageable) && _enemy.Target.TryGetComponent<PhotonView>(out var targetView))
        {
            // 데미지 처리
            //_enemy.Animator.SetTrigger("attack");

            //damageable.RPC_TakeDamage(_enemy.EnemyData.Damage);
            // 실제 적용할땐 이거로 대체
            //targetView.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, _enemy.EnemyData.Damage);
            _lastAttackTime = Time.time;
        }
    }
}