using UnityEngine;

public class TankerSkill : ISkill
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
            Debug.Log($"{Character.Name}의 TankerSkill은 아직 쿨타임 중입니다!");
            return;
        }
        Debug.Log($"{Character.Name} used TankerSkill! HP restored.");
        // Character.Heal(50);  // 이런 식으로 실제 캐릭터 로직 연동

        _timer = 0f;
    }
}
