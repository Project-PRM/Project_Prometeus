using UnityEngine;

/// <summary>
/// 단일 대상 타겟 스킬
/// </summary>
public class BuffSkill : IUnitTargetSkill
{
    private float _timer = 0f;

    public SkillData Data { get; set; }

    public void Update()
    {
        _timer += Time.deltaTime;
    }

    public GameObject GetIndicatorPrefab()
    {
        return Resources.Load<GameObject>($"Indicators/{Data.IndicatorPrefabName}");
    }

    public void Activate(CharacterBase user, CharacterBase target)
    {
        if (_timer < Data.Cooltime)
        {
            Debug.Log($"{user.Name} DummyPassive is on cooldown.");
            return;
        }
        // TODO : 같은/다른 팀 여부 판단 구현할것
        if (user.Team != target.Team)
        {
            Debug.Log($"{user.Name} tried to use BuffSkill on enemy {target.Name} — blocked.");
            return;
        }

        if (target.Behaviour.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.Heal(Data.BuffAmount);
        }

        _timer = 0f;
    }
}