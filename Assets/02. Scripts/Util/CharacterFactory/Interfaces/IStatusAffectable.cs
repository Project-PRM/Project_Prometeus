/// <summary>
/// IDamageable처럼, 상태이상을 받는 객체를 위한 인터페이스
/// </summary>
public interface IStatusAffectable
{
    public void ApplyEffect(IStatusEffect effect);
    public void RemoveEffect(IStatusEffect effect);
}
