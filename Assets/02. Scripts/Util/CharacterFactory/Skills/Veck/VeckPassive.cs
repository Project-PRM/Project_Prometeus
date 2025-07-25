using UnityEngine;

public class VeckPassive : IPermanentSkill
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
        buffMod.Add(EStatType.MoveSpeed, Data.BuffAmount[EStatType.BaseArmor.ToString()]);
        buffMod.Add(EStatType.MaxHealth, -Data.DebuffAmount[EStatType.MoveSpeed.ToString()]);
        Character.AddStatModifier(buffMod);
    }
}
