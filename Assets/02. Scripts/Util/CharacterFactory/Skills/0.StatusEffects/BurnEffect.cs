using System.Collections;
using UnityEngine;

public class BurnEffect : IStatusEffect
{
    public float Duration { get; private set; }
    public float Damage { get; private set; }

    private Coroutine _burn;

    public BurnEffect(float duration, float damage)
    {
        Duration = duration;
        Damage = damage;

        // duration동안 tick(0.5초)당 damage만큼 대미지를 준다
    }

    public void Apply(CharacterBase target)
    {
        if (target.Behaviour != null && _burn == null)
        {
            _burn = target.Behaviour.StartCoroutine(BurnRoutine(target));
        }
    }

    public void Remove(CharacterBase target)
    {
        target.Behaviour.StopCoroutine(_burn);
    }

    private IEnumerator BurnRoutine(CharacterBase target)
    {
        float elapsed = 0f;
        var tick = new WaitForSeconds(0.5f);

        while (elapsed < Duration)
        {
            target.TakeDamage(Damage); // DamageType은 예시입니다.
            yield return tick;
            elapsed += 0.5f;
        }

        _burn = null;
        Remove(target);
    }
}
