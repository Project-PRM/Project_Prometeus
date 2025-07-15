using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterBehaviour))]
public class CharacterStatusEffect : MonoBehaviour, IStatusAffectable
{
    private CharacterBehaviour _characterbehaviour;

    [Header("# Status Effects")]
    private List<StatusEffectInstance> _activeEffects = new();

    private void Awake()
    {
        _characterbehaviour = GetComponent<CharacterBehaviour>();
    }

    public void ApplyEffect(IStatusEffect newEffect)
    {
        newEffect.Apply(_characterbehaviour.GetCharacterBase());

        var coroutine = StartCoroutine(EffectDurationCoroutine(newEffect));
        var instance = new StatusEffectInstance(newEffect, coroutine, newEffect.Duration);

        _activeEffects.Add(instance);
    }

    public void RemoveEffect(IStatusEffect effect)
    {
        // 첫 번째로 매칭되는 인스턴스를 찾음(정확히 동일한 참조)
        var instance = _activeEffects.Find(e => e.Effect == effect);
        if (instance == null) return;

        // 코루틴 정지
        if (instance.Coroutine != null)
            StopCoroutine(instance.Coroutine);

        // 효과 제거 및 목록에서 제거
        effect.Remove(_characterbehaviour.GetCharacterBase());
        _activeEffects.Remove(instance);
    }

    // 특정 타입의 효과를 모두 제거
    public void RemoveAllEffectsOfType<T>() where T : IStatusEffect
    {
        var toRemove = _activeEffects.FindAll(e => e.Effect is T);

        foreach (var instance in toRemove)
        {
            if (instance.Coroutine != null)
                StopCoroutine(instance.Coroutine);

            instance.Effect.Remove(_characterbehaviour.GetCharacterBase());
            _activeEffects.Remove(instance);
        }
    }

    private IEnumerator EffectDurationCoroutine(IStatusEffect effect)
    {
        float remaining = effect.Duration;

        while (remaining > 0f)
        {
            remaining -= Time.deltaTime;
            yield return null;
        }

        effect.Remove(_characterbehaviour.GetCharacterBase());

        // 리스트에서 해당 인스턴스를 제거
        _activeEffects.RemoveAll(e => e.Effect == effect);
    }
}