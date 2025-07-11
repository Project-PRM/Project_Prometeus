public static class StatCalculator
{
    public static CharacterStats CalculateFinalStats(CharacterStats baseStats, EquipmentSet equipment)
    {
        CharacterStats result = new CharacterStats
        {
            MaxHealth = baseStats.MaxHealth,
            MaxMana = baseStats.MaxMana,
            BaseDamage = baseStats.BaseDamage,
            BaseArmor = baseStats.BaseArmor,
            BaseAttackCoolTime = baseStats.BaseAttackCoolTime,
            BaseAttackRange = baseStats.BaseAttackRange,
            MoveSpeed = baseStats.MoveSpeed,
            SprintSpeed = baseStats.SprintSpeed,
            MaxStamina = baseStats.MaxStamina,
            StaminaRegen = baseStats.StaminaRegen,
            SprintStaminaCost = baseStats.SprintStaminaCost,
            BaseVision = baseStats.BaseVision
        };

        foreach (var item in equipment.GetAllEquipped())
        {
            var mod = item.StatModifiers;

            result.MaxHealth += mod.MaxHealth;
            result.MaxMana += mod.MaxMana;
            result.BaseDamage += mod.BaseDamage;
            result.BaseArmor += mod.BaseArmor;
            result.BaseAttackCoolTime += mod.BaseAttackCoolTime;
            result.BaseAttackRange += mod.BaseAttackRange;
            result.MoveSpeed += mod.MoveSpeed;
            result.SprintSpeed += mod.SprintSpeed;
            result.MaxStamina += mod.MaxStamina;
            result.StaminaRegen += mod.StaminaRegen;
            result.SprintStaminaCost += mod.SprintStaminaCost;
            result.BaseVision += mod.BaseVision;
        }

        return result;
    }
}

