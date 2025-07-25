using UnityEngine;
using Firebase.Firestore;
using System.Collections.Generic;

[FirestoreData]
public class EnemyData
{
    [FirestoreProperty] public string Name { get; set; }
    [FirestoreProperty] public float MaxHealth { get; set; }
    [FirestoreProperty] public float Armor { get; set; }
    [FirestoreProperty] public float Speed { get; set; }
    [FirestoreProperty] public float SprintSpeed { get; set; }
    [FirestoreProperty] public float Damage { get; set; }
    [FirestoreProperty] public float AttackCoolTime { get; set; }
    [FirestoreProperty] public float AttackRange { get; set; }
    [FirestoreProperty] public float VisionRange { get; set; }
    [FirestoreProperty] public float DetectionRange { get; set; }
    [FirestoreProperty] public float ManaReward { get; set; }
    [FirestoreProperty] public Dictionary<string, float> DropItemRewards { get; set; }

    public EnemyData() { }

    public EnemyData(EnemyData other)
    {
        Name = other.Name;
        MaxHealth = other.MaxHealth;
        Armor = other.Armor;
        Speed = other.Speed;
        SprintSpeed = other.SprintSpeed;
        Damage = other.Damage;
        AttackCoolTime = other.AttackCoolTime;
        AttackRange = other.AttackRange;
        VisionRange = other.VisionRange;
        DetectionRange = other.DetectionRange;
        ManaReward = other.ManaReward;

        // 깊은 복사: Dictionary도 새 인스턴스로 생성
        DropItemRewards = other.DropItemRewards != null
            ? new Dictionary<string, float>(other.DropItemRewards)
            : new Dictionary<string, float>();
    }
}
