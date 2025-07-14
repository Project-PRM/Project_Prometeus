public interface IStatusAffectable
{
    void ApplyEffect(IStatusEffect effect);
    void RemoveEffect(IStatusEffect effect);
}
