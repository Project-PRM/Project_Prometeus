using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public static class JsonFileUtil
{
    /// <summary>
    /// 파일이 없으면 Resources에서 기본 JSON 텍스트 로드해서 파일 생성,
    /// 기본 JSON 리소스 없으면 빈 T의 인스턴스를 JSON으로 생성 후 저장함
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath">저장 경로 (예: Assets/ItemData.json)</param>
    /// <param name="defaultResourcePath">Resources 폴더 내 경로 (확장자 없이, 예: "DefaultItemData")</param>
    /// <returns></returns>
    public static T LoadFromJsonFile<T>(string filePath, string defaultResourcePath = null) where T : new()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"파일이 없습니다: {filePath}");

            // 1. Resources 폴더에서 기본 JSON 불러오기 시도
            if (!string.IsNullOrEmpty(defaultResourcePath))
            {
                TextAsset defaultJson = Resources.Load<TextAsset>(defaultResourcePath);
                if (defaultJson != null)
                {
                    File.WriteAllText(filePath, defaultJson.text);
                    Debug.Log($"Resources에서 기본 JSON을 복사하여 파일 생성: {filePath}");
                }
                else
                {
                    Debug.LogWarning($"기본 JSON 리소스를 찾을 수 없습니다: {defaultResourcePath}");
                    // 2. 기본 JSON 없으면 빈 객체를 JSON으로 저장
                    CreateEmptyJsonFile<T>(filePath);
                }
            }
            else
            {
                // 3. 경로 지정 안하면 빈 객체를 JSON으로 저장
                CreateEmptyJsonFile<T>(filePath);
            }
        }

        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<T>(json);
    }

    /// <summary>
    /// 빈 객체를 JSON으로 직렬화해서 파일로 저장
    /// </summary>
    private static void CreateEmptyJsonFile<T>(string filePath) where T : new()
    {
        T emptyInstance = new T();
        string emptyJson = JsonConvert.SerializeObject(emptyInstance, Formatting.Indented);
        File.WriteAllText(filePath, emptyJson);
        Debug.Log($"빈 {typeof(T).Name} JSON 파일 생성: {filePath}");
    }

    public static void SaveToJsonFile<T>(string filePath, T data)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }
}
