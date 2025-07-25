using UnityEngine;

public class StatusEffectInstance
{
    public IStatusEffect Effect { get; }
    public Coroutine Coroutine { get; }
    public float Duration { get; }

    public StatusEffectInstance(IStatusEffect effect, Coroutine coroutine, float duration)
    {
        Effect = effect;
        Coroutine = coroutine;
        Duration = duration;
    }
}
