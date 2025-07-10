using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Trace", story: "[Self] Navigate To [Target]", category: "Action", id: "cbd067d5082f460c97be9adda206df8d")]
public partial class TraceAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private NavMeshAgent _agent;

    protected override Status OnStart()
    {
        _agent = Self.Value.GetComponent<NavMeshAgent>();
        _agent.speed = 5f;
        _agent.SetDestination(Target.Value.transform.position);

        return Status.Running;
    }

}

