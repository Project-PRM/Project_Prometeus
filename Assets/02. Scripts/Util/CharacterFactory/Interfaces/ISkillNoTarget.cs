/// <summary>
/// 자기 자신에게만 영향을 주는 스킬(버프, 스탯 변화 등)
/// </summary>
public interface ISkillNoTarget : ISkill
{
    public void Activate(CharacterBase user);
}