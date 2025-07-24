using UnityEngine;

public class PatrolAction : IActionNode
{
    private EnemyController _enemy;

    public PatrolAction(EnemyController enemy) => _enemy = enemy;

    public ENodeState Evaluate()
    {
        Patrol();
        return ENodeState.Running;
    }

    public void Patrol()
    {
        Debug.Log("패트롤 중");
        // Waypoint 이동 등
    }
}
