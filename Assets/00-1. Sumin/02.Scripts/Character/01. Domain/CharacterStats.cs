using Firebase.Firestore;
using System;

[FirestoreData]
public class CharacterStats
{
    [FirestoreProperty] public float MaxHealth { get; set; }
    [FirestoreProperty] public float MaxMana { get; set; }
    [FirestoreProperty] public float BaseDamage { get; set; }
    [FirestoreProperty] public float BaseArmor { get; set; }
    [FirestoreProperty] public float BaseAttackCoolTime { get; set; }
    [FirestoreProperty] public float BaseAttackRange { get; set; }
    [FirestoreProperty] public float MoveSpeed { get; set; }
    [FirestoreProperty] public float SprintSpeed { get; set; }
    [FirestoreProperty] public float MaxStamina { get; set; }
    [FirestoreProperty] public float StaminaRegen { get; set; }
    [FirestoreProperty] public float SprintStaminaCost { get; set; }
    [FirestoreProperty] public float BaseVision { get; set; }

    // Firebase용 기본 생성자
    public CharacterStats() { }

    // 전체 필드 초기화 생성자
    public CharacterStats(
        float maxHealth,
        float maxMana,
        float baseDamage,
        float baseArmor,
        float baseAttackCoolTime,
        float baseAttackRange,
        float moveSpeed,
        float sprintSpeed,
        float maxStamina,
        float staminaRegen,
        float sprintStaminaCost,
        float baseVision)
    {
        MaxHealth = maxHealth;
        MaxMana = maxMana;
        BaseDamage = baseDamage;
        BaseArmor = baseArmor;
        BaseAttackCoolTime = baseAttackCoolTime;
        BaseAttackRange = baseAttackRange;
        MoveSpeed = moveSpeed;
        SprintSpeed = sprintSpeed;
        MaxStamina = maxStamina;
        StaminaRegen = staminaRegen;
        SprintStaminaCost = sprintStaminaCost;
        BaseVision = baseVision;
    }

    // 깊은 복사 생성자
    public CharacterStats(CharacterStats other)
    {
        MaxHealth = other.MaxHealth;
        MaxMana = other.MaxMana;
        BaseDamage = other.BaseDamage;
        BaseArmor = other.BaseArmor;
        BaseAttackCoolTime = other.BaseAttackCoolTime;
        BaseAttackRange = other.BaseAttackRange;
        MoveSpeed = other.MoveSpeed;
        SprintSpeed = other.SprintSpeed;
        MaxStamina = other.MaxStamina;
        StaminaRegen = other.StaminaRegen;
        SprintStaminaCost = other.SprintStaminaCost;
        BaseVision = other.BaseVision;
    }

    public float this[EStatType stat]
    {
        get
        {
            return stat switch
            {
                EStatType.MaxHealth => MaxHealth,
                EStatType.MaxMana => MaxMana,
                EStatType.BaseDamage => BaseDamage,
                EStatType.BaseArmor => BaseArmor,
                EStatType.BaseAttackCoolTime => BaseAttackCoolTime,
                EStatType.BaseAttackRange => BaseAttackRange,
                EStatType.MoveSpeed => MoveSpeed,
                EStatType.SprintSpeed => SprintSpeed,
                EStatType.MaxStamina => MaxStamina,
                EStatType.StaminaRegen => StaminaRegen,
                EStatType.SprintStaminaCost => SprintStaminaCost,
                EStatType.BaseVision => BaseVision,
                _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
            };
        }
        set
        {
            switch (stat)
            {
                case EStatType.MaxHealth: MaxHealth = value; break;
                case EStatType.MaxMana: MaxMana = value; break;
                case EStatType.BaseDamage: BaseDamage = value; break;
                case EStatType.BaseArmor: BaseArmor = value; break;
                case EStatType.BaseAttackCoolTime: BaseAttackCoolTime = value; break;
                case EStatType.BaseAttackRange: BaseAttackRange = value; break;
                case EStatType.MoveSpeed: MoveSpeed = value; break;
                case EStatType.SprintSpeed: SprintSpeed = value; break;
                case EStatType.MaxStamina: MaxStamina = value; break;
                case EStatType.StaminaRegen: StaminaRegen = value; break;
                case EStatType.SprintStaminaCost: SprintStaminaCost = value; break;
                case EStatType.BaseVision: BaseVision = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
            }
        }
    }
}
