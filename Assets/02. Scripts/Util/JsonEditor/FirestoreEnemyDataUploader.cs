#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

public static class FirestoreEnemyDataUploader
{
    private const string COLLECTION_NAME = "EnemyDatas";
    private const string JSON_NAME = "Datas/enemy_data"; // Resources/Datas/enemy_data.json
    private const string JSON_PATH = "Assets/Resources/Datas/enemy_data.json";

    [MenuItem("Tools/Enemy/Upload EnemyData To Firestore")]
    public static void Upload()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result != Firebase.DependencyStatus.Available)
            {
                Debug.LogError("Firebase 초기화 실패");
                return;
            }

            UploadFromJsonAsync().ContinueWithOnMainThread(uploadTask =>
            {
                if (uploadTask.IsCompletedSuccessfully)
                    Debug.Log("✅ Enemy 데이터 업로드 완료");
                else
                    Debug.LogError("🔥 업로드 실패: " + uploadTask.Exception);
            });
        });
    }

    private static async Task UploadFromJsonAsync()
    {
        EnsureJsonExists();

        TextAsset jsonText = Resources.Load<TextAsset>(JSON_NAME);
        if (jsonText == null)
        {
            Debug.LogError($"{JSON_NAME}.json 파일을 Resources에 넣어주세요.");
            return;
        }

        var enemies = JsonConvert.DeserializeObject<Dictionary<string, EnemyData>>(jsonText.text);
        var firestore = FirebaseFirestore.DefaultInstance;

        foreach (var kvp in enemies)
        {
            string docId = kvp.Key;
            EnemyData data = kvp.Value;

            try
            {
                var docRef = firestore.Collection(COLLECTION_NAME).Document(docId);
                await docRef.SetAsync(data);

                Debug.Log($"<color=green>[✓]</color> {docId} 업로드 완료");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[X] {docId} 업로드 실패: {e.Message}");
            }
        }
    }

    private static void EnsureJsonExists()
    {
        if (File.Exists(JSON_PATH))
            return;

        var dummy = new Dictionary<string, EnemyData>
        {
            {
                "Goblin", new EnemyData
                {
                    Name = "Goblin",
                    MaxHealth = 100,
                    Armor = 5,
                    Speed = 2,
                    Damage = 10,
                    AttackCoolTime = 1.5f,
                    AttackRange = 1,
                    VisionRange = 4,
                    DetectionRange = 6,
                    ManaReward = 3,
                    DropItemRewards = new Dictionary<string, float>
                    {
                        { "Gold", 0.5f },
                        { "Gem", 0.1f }
                    }
                }
            }
        };

        string json = JsonConvert.SerializeObject(dummy, Formatting.Indented);
        File.WriteAllText(JSON_PATH, json);

        AssetDatabase.Refresh();
        Debug.Log($"⚠️ {JSON_PATH} 자동 생성됨. 내용을 확인하세요.");
    }
}
#endif
