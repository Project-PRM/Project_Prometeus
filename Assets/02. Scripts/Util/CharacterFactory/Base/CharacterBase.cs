using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase
{
    public event Action<ECharacterEvent> OnEventOccurred; // 스킬 사용, 기본 공격 사용 등

    [Header("# Datas")]
    public string Name { get; private set; }
    public CharacterBehaviour Behaviour { get; private set; }
    public CharacterStats BaseStats { get; private set; } // Firebase 기반
    //public CharacterStats FinalStats => StatCalculator.CalculateFinalStats(BaseStats, Equipment);

    [Header("# INGAME")]
    public EquipmentSet Equipment { get; private set; }

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

    public void UseSkill(ESkillType type)
    {
        switch (type)
        {
            case ESkillType.BasicAttack:
                _basicAttack.Activate(this);
                RaiseEvent(ECharacterEvent.OnBasicAttack);
                break;
            //case ESkillType.Passive:
            //    _passive.Activate(this); 
            //    RaiseEvent(ECharacterEvent.OnSkillUsed);
            //    break;
            case ESkillType.Skill:
                _skill.Activate(this);
                RaiseEvent(ECharacterEvent.OnSkillUsed);
                break;
            case ESkillType.Ultimate:
                _ultimate.Activate(this);
                RaiseEvent(ECharacterEvent.OnSkillUsed);
                break;
            default:
                break;
        }
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

    public void BindPassiveEvents()
    {
        if (_passive is IEventReactiveSkill reactive)
        {
            OnEventOccurred += reactive.OnEvent;
        }
    }
}
