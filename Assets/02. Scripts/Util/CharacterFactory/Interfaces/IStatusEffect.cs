/// <summary>
/// 상태 이상 인터페이스
/// </summary>
public interface IStatusEffect
{
    public void Apply(CharacterBase target);
    public void Remove(CharacterBase target);
    public float Duration { get; }
}