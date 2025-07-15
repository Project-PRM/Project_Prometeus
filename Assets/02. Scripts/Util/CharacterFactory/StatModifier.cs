using System.Collections.Generic;

public class StatModifier
{
    private Dictionary<EStatType, float> _additives = new();
    private Dictionary<EStatType, float> _multipliers = new();

    public void Add(EStatType type, float value)
    {
        if (_additives.ContainsKey(type))
            _additives[type] += value;
        else
            _additives[type] = value;
    }

    public void Multiply(EStatType type, float multiplier)
    {
        if (_multipliers.ContainsKey(type))
            _multipliers[type] *= multiplier;
        else
            _multipliers[type] = multiplier;
    }
    
    /// <summary>
    /// 합연산 총량
    /// </summary>
    public float GetAdditive(EStatType type) =>
        _additives.TryGetValue(type, out var value) ? value : 0f;

    /// <summary>
    /// 곱연산 총량
    /// </summary>
    public float GetMultiplier(EStatType type) =>
        _multipliers.TryGetValue(type, out var value) ? value : 1f;

    public IEnumerable<EStatType> GetAllStatTypes()
    {
        var keys = new HashSet<EStatType>(_additives.Keys);
        keys.UnionWith(_multipliers.Keys);
        return keys;
    }
}

/*사용 예시
var baseStats = new CharacterStats(...);

// 장비 효과
var itemMod = new StatModifier();
itemMod.Add(EStatType.BaseDamage, 10f);
itemMod.Add(EStatType.MoveSpeed, 2f);

// 버프
var buffMod = new StatModifier();
buffMod.Multiply(EStatType.BaseDamage, 1.5f); // +50%
buffMod.Multiply(EStatType.MoveSpeed, 1.2f);  // +20%

var finalStats = StatCalculator.CalculateFinalStats(baseStats, new[] { itemMod, buffMod });
*/