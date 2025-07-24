using UnityEngine;

public class HealerUltimate : ISkill
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

    public void Activate()
    {
        if (_timer < Data.Cooltime)
        {
            Debug.Log($"{Character.Name}의 HealerUltimate은 아직 쿨타임 중입니다!");
            return;
        }
        // 더미 궁극기 스킬의 동작을 구현합니다.
        // 예시로, 캐릭터의 HP를 회복하는 로직을 추가할 수 있습니다.
        Debug.Log($"{Character.Name} used HealerUltimate! HP restored.");
        // Character.Heal(100);  // 실제 캐릭터 로직 연동

        _timer = 0f;
    }
}
