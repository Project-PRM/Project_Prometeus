using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using UnityEngine.Splines;
using Photon.Realtime;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Patrol", story: "[Self] Navigate To PatrolPosition", category: "Action", id: "94bb376163ae0aea3cd2ee4fc3dd7924")]
public partial class PatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    private NavMeshAgent _agent;
    private Animator _animator;
    private Vector3 _patrolPostion;
    private float _currentPatrolTime = 0f;
    private float _maxPatrolTime = 5f;

    protected override Status OnStart()
    {
        int jitterMin = 0;
        int jitterMax = 360;
        float patrolRadius = UnityEngine.Random.Range(2.5f, 6f);
        float patroljitter = UnityEngine.Random.Range(jitterMin, jitterMax);

        // BlackboardVariable<> 타입의 변수는 변수명.Value로 값을 설정하거나 불러올 수 있음.

        _patrolPostion = Self.Value.transform.position + GetPositionFromAngle(patrolRadius, patroljitter);
        _agent = Self.Value.GetComponent<NavMeshAgent>();
        _agent.SetDestination(_patrolPostion);
        _currentPatrolTime = Time.time;
        _animator = Self.Value.GetComponent<Animator>();
        _animator.SetBool("IsWalking", true);


        return Status.Running;
    }

    protected override void OnEnd()
    {
        _animator.SetBool("IsWalking", false);
    }

    protected override Status OnUpdate()
    {
        // 목표 도달 혹은 어떤 이유로 도달 못할시 이 시간이 지날경우 종료
        if((_patrolPostion - Self.Value.transform.position).sqrMagnitude < 0.1f || Time.time - _currentPatrolTime > _maxPatrolTime)
        {
          return Status.Success;
        }

        return Status.Running;
    }

    private Vector3 GetPositionFromAngle(float radius, float angle)
    {
        Vector3 postion = Vector3.zero;

        angle = DegreeToRadian(angle);

        postion.x = Mathf.Cos(angle) * radius;
        postion.z = Mathf.Cos(angle) * radius;

        return postion;
    }

    private float DegreeToRadian(float angle)
    {
        return Mathf.PI * angle / 180;
    }


}

