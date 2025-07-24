using UnityEngine;

/// <summary>
/// 특정 위치/방향을 향해 사용하는 스킬
/// </summary>
public interface ITargetableSkill : ISkill
{
    public void Activate(Vector3 target);
    public GameObject GetIndicatorPrefab();
}