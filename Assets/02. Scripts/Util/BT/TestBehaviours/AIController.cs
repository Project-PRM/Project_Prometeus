using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private const float TICK = 0.5f;

    public Transform player;
    public float sightRange = 10f;
    public float attackRange = 2f;

    private BtNode _btRoot;

    private float _timer;

    private void Start()
    {
        _btRoot = new Selector(new List<BtNode>
        {
            new Sequence(new List<BtNode>
            {
                new IsPlayerInAttackRange(this),
                new AttackAction(this)
            }),
            new Sequence(new List<BtNode>
            {
                new IsPlayerInSight(this),
                new ChaseAction(this)
            }),
            new PatrolAction(this)
        });
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < TICK)
        {
            return;
        }

        _btRoot.Evaluate();
    }

    public Transform FindNearestPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, sightRange);
        float minDist = float.MaxValue;
        Transform nearest = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = hit.transform;
                }
            }
        }

        return nearest;
    }

    public bool IsPlayerInSight()
    {
        player = FindNearestPlayer();
        return player != null;
    }

    public bool IsPlayerInAttackRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) < attackRange;
    }

    public void Patrol()
    {
        Debug.Log("패트롤 중");
        // Waypoint 이동 등
    }

    public void ChasePlayer()
    {
        Debug.Log("추격 중");
        // 플레이어 쪽으로 이동
    }

    public void AttackPlayer()
    {
        Debug.Log("공격!");
        // 애니메이션 실행 + 데미지
    }
}
