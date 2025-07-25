using UnityEngine;
using Firebase.Firestore;
using System.Collections.Generic;

[System.Serializable]
[FirestoreData]
public class SkillData
{
    [FirestoreProperty] public string SkillName { get; set; }
    [FirestoreProperty] public float Damage { get; set; }
    [FirestoreProperty] public Dictionary<EStatType, float> BuffAmount { get; set; }
    [FirestoreProperty] public Dictionary<EStatType, float> DebuffAmount { get; set; }
    [FirestoreProperty] public float Cost { get; set; }
    [FirestoreProperty] public float Cooltime { get; set; }
    [FirestoreProperty] public float Duration { get; set; }
    [FirestoreProperty] public float Radius { get; set; }
    [FirestoreProperty] public float MaxRange { get; set; }
    [FirestoreProperty] public float Speed { get; set; }
    [FirestoreProperty] public string ProjectilePrefabName { get; set; }
    [FirestoreProperty] public string SummonPrefabName { get; set; }
    [FirestoreProperty] public string IndicatorPrefabName { get; set; }

    public SkillData() { }
}
