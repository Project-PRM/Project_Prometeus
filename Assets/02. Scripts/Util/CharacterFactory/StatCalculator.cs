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
        foreach (EStatType type in Enum.GetValues(typeof(EStatType)))
        {
            float baseValue = stats[type];
            stats[type] = (baseValue + add[type]) * mul[type];
        }
    }
}