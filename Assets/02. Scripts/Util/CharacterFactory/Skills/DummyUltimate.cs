using UnityEngine;

[Skill("DummyUltimate")]
public class DummyUltimate : ISkill
{
    private float _timer = 0f;

    public SkillData Data { get; set; }

    public void Update()
    {
        _timer += Time.deltaTime;
    }

    public void Activate(CharacterBase user)
    {
        // 더미 궁극기 스킬의 동작을 구현합니다.
        // 예시로, 캐릭터의 HP를 회복하는 로직을 추가할 수 있습니다.
        Debug.Log($"{user.Name} used DummyUltimate! HP restored.");
        // user.Heal(100);  // 실제 캐릭터 로직 연동
    }
}