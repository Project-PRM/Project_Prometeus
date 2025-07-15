public class SlowEffect : IStatusEffect
{
    public float Duration { get; private set; }
    private StatModifier _modifier;

    public SlowEffect(float duration, float slowMultiplier)
    {
        Duration = duration;

        _modifier = new StatModifier();
        _modifier.Multiply(EStatType.MoveSpeed, slowMultiplier); // 예: 0.5f → 50% 감소
    }

    public void Apply(CharacterBase target)
    {
        target.AddStatModifier(_modifier);
    }

    public void Remove(CharacterBase target)
    {
        target.RemoveStatModifier(_modifier);
    }
}
