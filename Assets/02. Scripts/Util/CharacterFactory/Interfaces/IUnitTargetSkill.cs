using UnityEngine;

/// <summary>
/// 특정 대상을 지정해 사용하는 스킬(버프, 스탯 추가 등)
/// </summary>
public interface IUnitTargetSkill : ISkill
{
    public void Activate(CharacterBase target);
    public GameObject GetIndicatorPrefab();
}