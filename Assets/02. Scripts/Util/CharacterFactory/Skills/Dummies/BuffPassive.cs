using UnityEngine;

/// <summary>
/// 영구 스탯 변화형 스킬
/// </summary>
public class BuffPassive : IPermanentSkill
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
        var buffMod = new StatModifier();
        buffMod.Add(EStatType.MoveSpeed, Data.BuffAmount[EStatType.MoveSpeed.ToString()]);
        buffMod.Add(EStatType.MaxHealth, Data.BuffAmount[EStatType.MaxHealth.ToString()]);
        Character.AddStatModifier(buffMod);
    }
}