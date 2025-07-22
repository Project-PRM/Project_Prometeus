#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class FirestoreItemDataUploader
{
    private class ItemDataMap : Dictionary<string, ItemData> { }

    [MenuItem("Tools/Item/Upload ItemData To Firestore")]
    public static void UploadItemData()
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
                    Debug.Log("✅ 모든 아이템 데이터 업로드 완료");
                else
                    Debug.LogError("🔥 업로드 실패: " + uploadTask.Exception);
            });
        });
    }

    private static async Task UploadFromJsonAsync()
    {
        var firestore = FirebaseFirestore.DefaultInstance;

        // Resources에서 JSON 로드
        TextAsset jsonText = Resources.Load<TextAsset>("Datas/item_data");
        if (jsonText == null)
        {
            Debug.LogError("item_data.json 파일을 Resources 폴더에 넣어주세요.");
            return;
        }

        var map = JsonUtilityWrapper.FromJson<ItemDataMap>(jsonText.text);

        foreach (var kvp in map)
        {
            try
            {
                ItemData data = kvp.Value;
                string docId = data.Name;  // 이름 또는 ID 필드 기준 문서명

                DocumentReference docRef = firestore.Collection("ItemDatas").Document(docId);
                await docRef.SetAsync(data);

                Debug.Log($"<color=green>[✓]</color> {docId} 업로드 완료");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[X] {kvp.Key} 업로드 실패: {e.Message}");
            }
        }
    }

    // JsonUtility는 Dictionary 직렬화 지원 안 함 → Newtonsoft.Json 사용
    private static class JsonUtilityWrapper
    {
        public static Dictionary<string, ItemData> FromJson<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ItemData>>(json);
        }
    }
}
#endif
