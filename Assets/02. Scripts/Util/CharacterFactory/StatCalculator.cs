using System.Collections.Generic;
using System;

public static class StatCalculator
{
    public static CharacterStats CalculateFinalStats(CharacterStats baseStats, IEnumerable<StatModifier> modifiers)
    {
        CharacterStats final = new CharacterStats(baseStats); // 깊은 복사

        // 누적된 덧셈/곱셈 값을 모으기
        Dictionary<EStatType, float> totalAdd = new();
        Dictionary<EStatType, float> totalMul = new();

        foreach (var type in Enum.GetValues(typeof(EStatType)))
        {
            totalAdd[(EStatType)type] = 0f;
            totalMul[(EStatType)type] = 1f;
        }

        foreach (var modifier in modifiers)
        {
            foreach (var statType in modifier.GetAllStatTypes())
            {
                totalAdd[statType] += modifier.GetAdditive(statType);
                totalMul[statType] *= modifier.GetMultiplier(statType);
            }
        }

        // 적용
        Apply(final, totalAdd, totalMul);

        return final;
    }

    private static void Apply(CharacterStats stats,
                               Dictionary<EStatType, float> add,
                               Dictionary<EStatType, float> mul)
    {
        stats.MaxHealth = (stats.MaxHealth + add[EStatType.MaxHealth]) * mul[EStatType.MaxHealth];
        stats.MaxMana = (stats.MaxMana + add[EStatType.MaxMana]) * mul[EStatType.MaxMana];
        stats.BaseDamage = (stats.BaseDamage + add[EStatType.BaseDamage]) * mul[EStatType.BaseDamage];
        stats.BaseArmor = (stats.BaseArmor + add[EStatType.BaseArmor]) * mul[EStatType.BaseArmor];
        stats.BaseAttackCoolTime = (stats.BaseAttackCoolTime + add[EStatType.BaseAttackCoolTime]) * mul[EStatType.BaseAttackCoolTime];
        stats.BaseAttackRange = (stats.BaseAttackRange + add[EStatType.BaseAttackRange]) * mul[EStatType.BaseAttackRange];
        stats.MoveSpeed = (stats.MoveSpeed + add[EStatType.MoveSpeed]) * mul[EStatType.MoveSpeed];
        stats.SprintSpeed = (stats.SprintSpeed + add[EStatType.SprintSpeed]) * mul[EStatType.SprintSpeed];
        stats.MaxStamina = (stats.MaxStamina + add[EStatType.MaxStamina]) * mul[EStatType.MaxStamina];
        stats.StaminaRegen = (stats.StaminaRegen + add[EStatType.StaminaRegen]) * mul[EStatType.StaminaRegen];
        stats.SprintStaminaCost = (stats.SprintStaminaCost + add[EStatType.SprintStaminaCost]) * mul[EStatType.SprintStaminaCost];
        stats.BaseVision = (stats.BaseVision + add[EStatType.BaseVision]) * mul[EStatType.BaseVision];
    }
}
