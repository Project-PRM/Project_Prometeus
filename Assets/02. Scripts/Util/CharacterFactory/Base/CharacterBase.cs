using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase
{
    public event Action<ECharacterEvent> OnEventOccurred; // 스킬 사용, 기본 공격 사용 등

    [Header("# Components")]
    public CharacterBehaviour Behaviour { get; private set; }

    [Header("# Datas")]
    public string Name { get; private set; }
    public string Team { get; private set; }
    public float CurrentHealth { get; private set; }
    public CharacterStats BaseStats { get; private set; } // Firebase 기반
    private bool _isDirty = true;
    private CharacterStats _cachedFinalStats;
    public CharacterStats FinalStats
    {
        get
        {
            if (_isDirty || _cachedFinalStats == null)
            {
                _cachedFinalStats = StatCalculator.CalculateFinalStats(BaseStats, _modifiers);
                _isDirty = false;
            }
            return _cachedFinalStats;
        }
    }

    [Header("# INGAME")]
    public EquipmentSet Equipment { get; private set; }
    private List<StatModifier> _modifiers = new();
    public void AddStatModifier(StatModifier mod)
    {
        _modifiers.Add(mod);
        _isDirty = true;
    }

    public void RemoveStatModifier(StatModifier mod)
    {
        _modifiers.Remove(mod);
        _isDirty = true;
    }

    private ISkill _basicAttack;
    private ISkill _passive;
    private ISkill _skill;
    private ISkill _ultimate;

    public CharacterBase(CharacterBehaviour behaviour, string name, string team, ISkill basicAttack, ISkill passive, ISkill skill, ISkill ultimate, CharacterStats baseStats)
    {
        Behaviour = behaviour;
        Name = name;
        Team = team;
        _basicAttack = basicAttack;
        _passive = passive;
        _skill = skill;
        _ultimate = ultimate;
        BaseStats = baseStats;
        CurrentHealth = BaseStats.MaxHealth;
        BindPassiveEvents();
    }

    public ISkill GetSkill(ESkillType type)
    {
        return type switch
        {
            ESkillType.BasicAttack => _basicAttack,
            ESkillType.Passive => _passive,
            ESkillType.Skill => _skill,
            ESkillType.Ultimate => _ultimate,
            _ => null
        };
    }

    public void UseSkill(ESkillType type, CharacterBase target = null, Vector3? position = null)
    {
        ISkill skill = type switch
        {
            ESkillType.BasicAttack => _basicAttack,
            ESkillType.Passive => _passive,
            ESkillType.Skill => _skill,
            ESkillType.Ultimate => _ultimate,
            _ => null
        };

        if (skill == null) return;

        if (skill is IUnitTargetSkill unitTargetSkill && target != null)
        {
            unitTargetSkill.Activate(this, target);
        }
        else if (skill is ITargetableSkill targetableSkill && position.HasValue)
        {
            targetableSkill.Activate(this, position.Value);
        }
        else if (skill is ISkillNoTarget skillNoTarget)
        {
            skillNoTarget.Activate(this);
        } 
        else
        {
            Debug.LogWarning($"Skill {type} activation failed: parameters mismatch or unsupported skill type.");
            return;
        }

        // 이벤트 발생
        if (type == ESkillType.BasicAttack)
            RaiseEvent(ECharacterEvent.OnBasicAttack);
        else
            RaiseEvent(ECharacterEvent.OnSkillUsed);
    }

    public void Update()
    {
        _basicAttack.Update();
        _passive.Update();
        _skill.Update();
        _ultimate.Update();
    }

    public void RaiseEvent(ECharacterEvent characterEvent)
    {
        OnEventOccurred?.Invoke(characterEvent);
    }

    private void BindPassiveEvents()
    {
        if (_passive is IEventReactiveSkill reactive)
        {
            OnEventOccurred += reactive.OnEvent;
        }
    }

    public void TakeDamage(float damage)
    {
        float finalDamage = DamageCalculator.CalculateDamage(damage, FinalStats.BaseArmor);
        CurrentHealth -= finalDamage;
        Debug.Log($"{CurrentHealth} is currenthealth | {finalDamage} is finalDamage");
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
            RaiseEvent(ECharacterEvent.OnDeath);
            return;
        }
        Debug.Log("TAKE DAMAGE HAS BEEN CALLED");
        RaiseEvent(ECharacterEvent.OnDamaged);
    }

    public void Heal(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > FinalStats.MaxHealth)
        {
            CurrentHealth = FinalStats.MaxHealth;
        }
    }
}