using UnityEngine;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EnemyDataRepository
{
    private FirebaseFirestore _db => FirebaseInitialize.DB;

    public Dictionary<string, EnemyData> Enemies { get; private set; } = new();

    public bool Initialized { get; private set; } = false;

    public async Task InitializeAsync()
    {
        if (Initialized) return;
        Initialized = true;
        Enemies.Clear();
        await GetAllEnemyDataAsync();
    }

    public async Task<EnemyData> GetEnemyDataAsync(string enemyName)
    {
        if (Enemies.TryGetValue(enemyName, out var cached))
            return cached;

        await FirebaseInitialize.WaitForInitializationAsync();

        var docRef = _db.Collection("EnemyDatas").Document(enemyName);
        var snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            var enemy = snapshot.ConvertTo<EnemyData>();
            Enemies[enemy.Name] = enemy;
            return enemy;
        }
        else
        {
            Debug.LogError("적 데이터가 존재하지 않습니다.");
            return null;
        }
    }

    public async Task GetAllEnemyDataAsync()
    {
        await FirebaseInitialize.WaitForInitializationAsync();

        var collectionRef = _db.Collection("EnemyDatas");
        var querySnapshot = await collectionRef.GetSnapshotAsync();

        foreach(var docSnap in querySnapshot.Documents)
        {
            if (docSnap.Exists)
            {
                var enemyData = docSnap.ConvertTo<EnemyData>();
                Enemies[docSnap.Id] = enemyData;
            }
        }
    }
}
