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
    [FirestoreProperty] public float Damage { get; set; }
    [FirestoreProperty] public float AttackCoolTime { get; set; }
    [FirestoreProperty] public float AttackRange { get; set; }
    [FirestoreProperty] public float VisionRange { get; set; }
    [FirestoreProperty] public float DetectionRange { get; set; }
    [FirestoreProperty] public float ManaReward { get; set; }
    [FirestoreProperty] public Dictionary<string, float> DropItemRewards { get; set; }
}
