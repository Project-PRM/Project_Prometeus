using Firebase.Firestore;

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
}
