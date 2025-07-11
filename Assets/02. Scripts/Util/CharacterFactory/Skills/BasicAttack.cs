using UnityEngine;

[Skill("BasicAttack")]
public class BasicAttack : ISkill
{
    private float _timer = 0f;

    public SkillData Data { get; set; }

    public void Update()
    {
        _timer += Time.deltaTime;
    }

    public void Activate(CharacterBase user)
    {
        // BasicAttack 스킬의 동작을 구현합니다.
        // 예시로, 공격력과 범위 등을 설정할 수 있습니다.
        Debug.Log($"{user.Name}이(가) 기본 공격을 사용했습니다!");

        // 실제 게임 로직에 맞게 공격력, 범위 등을 적용하는 코드를 추가해야 합니다.
    }
}