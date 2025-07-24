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
        if(Time.time < _lastAttackTime + _enemy.EnemyData.AttackCoolTime)
        {
            return ENodeState.Running;
        }
        AttackPlayer();
        return ENodeState.Success;
    }

    public void AttackPlayer()
    {
        if (_enemy.Target == null)
            return;

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