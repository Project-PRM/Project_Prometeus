using UnityEngine;

[Skill("DummySkill")]
public class DummySkill : ISkill
{
    private float _timer = 0f;

    public SkillData Data { get; set; }
    public CharacterBase Character { get; set; }
    public void SetOwner(CharacterBase character)
    {
        Character = character;
    }

    public void Update()
    {
        _timer += Time.deltaTime;
    }

    public GameObject GetIndicatorPrefab()
    {
        return Resources.Load<GameObject>($"Indicators/{Data.IndicatorPrefabName}");
    }

    public void Activate(CharacterBase user)
    {
        if(_timer < Data.Cooltime)
        {
            Debug.Log($"{user.Name}의 DummySkill은 아직 쿨타임 중입니다!");
            return;
        }
        Debug.Log($"{user.Name} used DummySkill! HP restored.");
        // user.Heal(50);  // 이런 식으로 실제 캐릭터 로직 연동

        _timer = 0f;
    }
}
