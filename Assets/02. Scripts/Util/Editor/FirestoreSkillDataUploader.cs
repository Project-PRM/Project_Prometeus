#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

public static class FirestoreSkillDataUploader
{
    private class SkillDataMap : Dictionary<string, SkillData> { }

    [MenuItem("Tools/Skill/Upload SkillData To Firestore")]
    public static void UploadSkillData()
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
                    Debug.Log("✅ 모든 스킬 데이터 업로드 완료");
                else
                    Debug.LogError("🔥 업로드 실패: " + uploadTask.Exception);
            });
        });
    }

    private static async Task UploadFromJsonAsync()
    {
        var firestore = FirebaseFirestore.DefaultInstance;

        // Resources에서 JSON 로드
        TextAsset jsonText = Resources.Load<TextAsset>("skill_data");
        if (jsonText == null)
        {
            Debug.LogError("skill_data.json 파일을 Resources 폴더에 넣어주세요.");
            return;
        }

        var map = JsonUtilityWrapper.FromJson<SkillDataMap>(jsonText.text);

        foreach (var kvp in map)
        {
            try
            {
                SkillData data = kvp.Value;
                string docId = data.SkillName;

                DocumentReference docRef = firestore.Collection("SkillDatas").Document(docId);
                await docRef.SetAsync(data);

                Debug.Log($"<color=green>[✓]</color> {docId} 업로드 완료");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[X] {kvp.Key} 업로드 실패: {e.Message}");
            }
        }
    }

    // JsonUtility는 Dictionary 직렬화 지원 안 함 → 커스텀 파서
    private static class JsonUtilityWrapper
    {
        public static Dictionary<string, SkillData> FromJson<T>(string json)
        {
            // JsonUtility로는 Dictionary 파싱 불가 → Newtonsoft.Json 쓰거나 수동 처리
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, SkillData>>(json);
        }
    }
}
#endif
