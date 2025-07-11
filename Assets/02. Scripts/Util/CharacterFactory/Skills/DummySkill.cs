using UnityEngine;

[Skill("DummySkill")]
public class DummySkill : ISkill
{
    private float _timer = 0f;

    public SkillData Data { get; set; }

    public void Update()
    {
        _timer += Time.deltaTime;
    }

    public void Activate(CharacterBase user)
    {
        Debug.Log($"{user.Name} used DummySkill! HP restored.");
        // user.Heal(50);  // 이런 식으로 실제 캐릭터 로직 연동
    }
}
