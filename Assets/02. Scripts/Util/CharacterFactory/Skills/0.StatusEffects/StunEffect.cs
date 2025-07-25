public class StunEffect : IStatusEffect
{
    public float Duration { get; private set; }

    public StunEffect(float duration)
    {
        Duration = duration;
    }

    public void Apply(CharacterBase target)
    {
        //// 스턴 효과 적용 로직
        //target.Behaviour.IsStunned = true;
        //target.OnEventOccurred?.Invoke(ECharacterEvent.OnStunned);
        
        //// 예시: 스턴 지속 시간 후에 제거
        //target.Behaviour.StartCoroutine(RemoveAfterDuration(target, Duration));
    }

    public void Remove(CharacterBase target)
    {
        //// 스턴 효과 제거 로직
        //target.Behaviour.IsStunned = false;
        //target.OnEventOccurred?.Invoke(ECharacterEvent.OnStunRemoved);
    }

    private System.Collections.IEnumerator RemoveAfterDuration(CharacterBase target, float duration)
    {
        yield return new UnityEngine.WaitForSeconds(duration);
        Remove(target);
    }
}

/*public class StunEffect : IStatusEffect
{
    public float Duration { get; private set; }

    public StunEffect(float duration)
    {
        Duration = duration;
    }

    public void Apply(CharacterBase target)
    {
        target.Behaviour.SetStunned(true);
        Debug.Log($"{target.Name} is stunned for {Duration} seconds.");
    }

    public void Remove(CharacterBase target)
    {
        target.Behaviour.SetStunned(false);
        Debug.Log($"{target.Name} is no longer stunned.");
    }
}
*/