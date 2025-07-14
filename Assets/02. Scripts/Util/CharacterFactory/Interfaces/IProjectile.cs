using UnityEngine;

/// <summary>
/// 투사체를 위한 인터페이스
/// </summary>
public interface IProjectile
{
    public void SetData(SkillData data, CharacterBase character, Vector3? direction = null);
}