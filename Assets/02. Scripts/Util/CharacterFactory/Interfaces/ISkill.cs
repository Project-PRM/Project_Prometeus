using UnityEngine;

/// <summary>
/// 기본 스킬 인터페이스 -> 이거 말고 자식들 써야함
/// </summary>
public interface ISkill
{
    public void Update();
    public SkillData Data { get; set; }
    GameObject GetIndicatorPrefab();
}