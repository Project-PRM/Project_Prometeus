using UnityEngine;

/// <summary>
/// 소환오브젝트용
/// </summary>
public interface ISummonObject
{
    public void SetData(SkillData data, CharacterBase character, CharacterBase target = null);
}
