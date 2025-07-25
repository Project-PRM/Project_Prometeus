public class ArmorBuffEffect : IStatusEffect
{
    public float Duration { get; private set; }
    private float _armorIncrease;

    public ArmorBuffEffect(float amount, float duration)
    {
        _armorIncrease = amount;
        Duration = duration;
    }

    public void Apply(CharacterBase target)
    {
        StatModifier mod = new StatModifier();
        mod.Add(EStatType.BaseArmor, _armorIncrease);
        target.AddStatModifier(mod);
    }

    public void Remove(CharacterBase target)
    {
        StatModifier mod = new StatModifier();
        mod.Add(EStatType.BaseArmor, _armorIncrease);
        target.RemoveStatModifier(mod);
    }
}
