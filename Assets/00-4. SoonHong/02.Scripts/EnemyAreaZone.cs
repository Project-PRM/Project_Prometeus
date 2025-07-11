using System.Collections.Generic;
using UnityEngine;

public class EnemyAreaZone : MonoBehaviour
{
    [Header("Area Settings")]
    public float AreaRadius = 10f;
    public Color GizmoColor = new Color(0, 1, 0, 0.25f); // 반투명 녹색
    public List<GameObject> PatrolPoints;

    private void OnDrawGizmos()
    {
        Gizmos.color = GizmoColor;
        Gizmos.DrawSphere(transform.position, AreaRadius);

        Gizmos.DrawWireSphere(transform.position, AreaRadius);
    }
}
