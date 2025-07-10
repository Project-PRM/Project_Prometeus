using UnityEngine;

public class DummySkill : ISkill
{
    // 이게 필수
    static DummySkill()
    {
        SkillFactory.Register("DummySkill", () => new DummySkill());
    }

    public void Activate(CharacterBase user)
    {
        Debug.Log($"{user.Name} used DummySkill! HP restored.");
        // user.Heal(50);  // 이런 식으로 실제 캐릭터 로직 연동
    }
}
