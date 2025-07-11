using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckTargetDetect", story: "Compare values of [CurrentDistance] and [TraceDistance]", category: "Conditions", id: "500f0f90c2c0644187dfc9ed99f2c37e")]
public partial class CheckTargetDetectCondition : Condition
{
    [SerializeReference] public BlackboardVariable<float> CurrentDistance;
    [SerializeReference] public BlackboardVariable<float> TraceDistance;

    public override bool IsTrue()
    {
        if (CurrentDistance.Value <= TraceDistance.Value)
        {
            return true;
        }
        return false;
    }

}
