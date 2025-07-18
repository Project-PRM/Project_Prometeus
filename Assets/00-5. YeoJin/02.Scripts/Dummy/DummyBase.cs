using System;
using System.Collections.Generic;
using UnityEngine;

public class DummyBase
{
    public event Action<ECharacterEvent> OnEventOccurred; // 스킬 사용, 기본 공격 사용 등

    [Header("# Components")]
    public DummyBehaviour Behaviour { get; private set; }

    [Header("# Datas")]
    public string Name { get; private set; }
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

    public DummyBase(DummyBehaviour behaviour, string name,  CharacterStats baseStats)
    {
        Behaviour = behaviour;
        Name = name;
        BaseStats = baseStats;
        CurrentHealth = BaseStats.MaxHealth;
    }

    private void RaiseEvent(ECharacterEvent characterEvent)
    {
        OnEventOccurred?.Invoke(characterEvent);
    }

    public void TakeDamage(float damage)
    {
        float finalDamage = DamageCalculator.CalculateDamage(damage, FinalStats.BaseArmor);
        CurrentHealth -= finalDamage;
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
            RaiseEvent(ECharacterEvent.OnDeath);
            return;
        }
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