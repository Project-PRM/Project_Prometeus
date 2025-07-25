using UnityEngine;

public class SpawnerPassive : IPermanentSkill
{
    public SkillData Data { get; set; }
    public CharacterBase Character { get; set; }
    public void SetOwner(CharacterBase character)
    {
        Character = character;
    }

    public void Update()
    {

    }

    public GameObject GetIndicatorPrefab()
    {
        return Resources.Load<GameObject>($"Indicators/{Data.IndicatorPrefabName}");
    }

    public void Activate()
    {
        Debug.Log($"{Character.Name} activated SpawnerPassive.");

        // 소환한 물체 근처 시야 - 오브젝트 자체에서 가능
    }
}
