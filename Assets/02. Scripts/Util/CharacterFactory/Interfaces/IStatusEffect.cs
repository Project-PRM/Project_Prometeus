public interface IStatusEffect
{
    public void Apply(CharacterBase target);
    public void Remove(CharacterBase target);
    public float Duration { get; }
}


public enum ECCType
{
    Stun,
    Slow,
}