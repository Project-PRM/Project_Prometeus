using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBTBase : MonoBehaviour
{
    [Header("영역 기준 오브젝트")]
    public EnemyAreaZone AreaObject;

    [Header("순찰 장소")]
    public List<GameObject> PatrolPoints;

    [Header("Enemy 스텟")]
    public EnemyStat EnemyStat;

    private GameObject _target;
    private NavMeshAgent _navMeshAgent;
    private BehaviorGraphAgent _behaviorAgent;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _behaviorAgent = GetComponent<BehaviorGraphAgent>();

        Init();
    }
    private void Init()
    {
        if (PatrolPoints != null && PatrolPoints.Count > 0)
        {
            _behaviorAgent.SetVariableValue("PatrolPoints", PatrolPoints);
        }
        _behaviorAgent.SetVariableValue("AreaObject", AreaObject.gameObject);
        _behaviorAgent.SetVariableValue("MoveSpeed", EnemyStat.MoveSpeed);
        _behaviorAgent.SetVariableValue("AttackDistance", EnemyStat.AttackDistance);
        _behaviorAgent.SetVariableValue("AreaRadius", AreaObject.AreaRadius);
        _behaviorAgent.SetVariableValue("IsMasterClient", Photon.Pun.PhotonNetwork.IsMasterClient);
    }

    public void Init(List<GameObject> patrolPoints)
    {
        _behaviorAgent.SetVariableValue("PatrolPoints", patrolPoints);
    }

    public void TargetSetup(GameObject target)
    {
        _target = target;
        _behaviorAgent.SetVariableValue("Target", target);
        _behaviorAgent.SetVariableValue("IsTargetDetected", true);
        Debug.Log("TargetOn");
    }
 
}
