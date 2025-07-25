using UnityEngine;

public class BotData
{
    public string Name;
    public float MaxHealth;
    public float MaxMana;
    public float BaseDamage;
    public float BaseArmor;
    public float BaseAttackCoolTime;
    public float MoveSpeed;
    public float SprintSpeed;
    public float MaxStamina;
    public float StaminaRegen;
    public float SprintStaminaCost;

    public float AttackRange;
    public float VisionRange;
    public float DetectionRange;
        
    // ✅ 전체 필드 초기화 생성자
    public BotData(
        string name,
        float maxHealth,
        float maxMana,
        float baseDamage,
        float baseArmor,
        float baseAttackCoolTime,
        float moveSpeed,
        float sprintSpeed,
        float maxStamina,
        float staminaRegen,
        float sprintStaminaCost,
        float attackRange,
        float visionRange,
        float detectionRange)
    {
        Name = name;
        MaxHealth = maxHealth;
        MaxMana = maxMana;
        BaseDamage = baseDamage;
        BaseArmor = baseArmor;
        BaseAttackCoolTime = baseAttackCoolTime;
        MoveSpeed = moveSpeed;
        SprintSpeed = sprintSpeed;
        MaxStamina = maxStamina;
        StaminaRegen = staminaRegen;
        SprintStaminaCost = sprintStaminaCost;
        AttackRange = attackRange;
        VisionRange = visionRange;
        DetectionRange = detectionRange;
    }

    // ✅ 깊은 복사 생성자
    public BotData(BotData other)
    {
        Name = string.Copy(other.Name);
        MaxHealth = other.MaxHealth;
        MaxMana = other.MaxMana;
        BaseDamage = other.BaseDamage;
        BaseArmor = other.BaseArmor;
        BaseAttackCoolTime = other.BaseAttackCoolTime;
        MoveSpeed = other.MoveSpeed;
        SprintSpeed = other.SprintSpeed;
        MaxStamina = other.MaxStamina;
        StaminaRegen = other.StaminaRegen;
        SprintStaminaCost = other.SprintStaminaCost;
        AttackRange = other.AttackRange;
        VisionRange = other.VisionRange;
        DetectionRange = other.DetectionRange;
    }
}
