using UnityEngine;

/// <summary>
/// 영구 스탯 변화형 스킬
/// </summary>
public class BuffPassive : IPermanentSkill
{
    public SkillData Data { get; set; }

    public void Update()
    {
    }

    public GameObject GetIndicatorPrefab()
    {
        return Resources.Load<GameObject>($"Indicators/{Data.IndicatorPrefabName}");
    }

    public void Activate(CharacterBase character)
    {
        var buffMod = new StatModifier();
        buffMod.Add(EStatType.MoveSpeed, Data.BuffAmount);
        buffMod.Add(EStatType.MaxHealth, Data.BuffAmount);
        character.AddStatModifier(buffMod);
    }
}