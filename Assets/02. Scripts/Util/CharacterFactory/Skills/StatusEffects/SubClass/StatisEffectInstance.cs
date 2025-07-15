using UnityEngine;

public class StatusEffectInstance
{
    public IStatusEffect Effect;
    public Coroutine Coroutine;
    public float RemainingTime;

    public StatusEffectInstance(IStatusEffect effect, Coroutine coroutine, float duration)
    {
        Effect = effect;
        Coroutine = coroutine;
        RemainingTime = duration;
    }
}