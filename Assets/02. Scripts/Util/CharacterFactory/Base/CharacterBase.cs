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
    public CharacterStats BaseStats { get; private set; } // Firebase 기반
    public CharacterStats FinalStats => StatCalculator.CalculateFinalStats(BaseStats, _modifiers);

    [Header("# INGAME")]
    public EquipmentSet Equipment { get; private set; }
    private List<StatModifier> _modifiers = new();
    public void AddStatModifier(StatModifier mod) => _modifiers.Add(mod);
    public void RemoveStatModifier(StatModifier mod) => _modifiers.Remove(mod);

    private ISkill _basicAttack;
    private ISkill _passive;
    private ISkill _skill;
    private ISkill _ultimate;

    public CharacterBase(CharacterBehaviour behaviour, string name, ISkill basicAttack, ISkill passive, ISkill skill, ISkill ultimate, CharacterStats baseStats)
    {
        Behaviour = behaviour;
        Name = name;
        _basicAttack = basicAttack;
        _passive = passive;
        _skill = skill;
        _ultimate = ultimate;
        BaseStats = baseStats;

        BindPassiveEvents();
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

    private void RaiseEvent(ECharacterEvent characterEvent)
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
}
