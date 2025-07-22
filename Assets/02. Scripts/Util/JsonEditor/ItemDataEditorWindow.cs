#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
public class ItemDataJsonEditorWindow : EditorWindow
{
    private Dictionary<string, ItemData> _items = new();
    private string _jsonPath = "Assets/Resources/Datas/item_data.json";
    private Vector2 _scrollPos;

    [MenuItem("Tools/Item/ItemData JSON Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<ItemDataJsonEditorWindow>("ItemData JSON Editor");
        window.LoadJson();
    }

    private void LoadJson()
    {
        if (File.Exists(_jsonPath))
        {
            string json = File.ReadAllText(_jsonPath);
            _items = JsonConvert.DeserializeObject<Dictionary<string, ItemData>>(json);
            Debug.Log($"✅ JSON 불러오기 성공: {_jsonPath}");
        }
        else
        {
            Debug.LogWarning($"⚠️ JSON 파일을 찾을 수 없습니다: {_jsonPath}");
            _items = new Dictionary<string, ItemData>();
        }
    }

    private void OnGUI()
    {
        if (_items == null || _items.Count == 0)
        {
            EditorGUILayout.HelpBox("아이템 데이터가 없습니다.", MessageType.Info);
        }

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        List<string> toRemove = new List<string>();
        List<(string oldKey, string newKey)> renameList = new List<(string, string)>();

        foreach (var oldKey in new List<string>(_items.Keys))
        {
            var item = _items[oldKey];

            EditorGUILayout.BeginVertical("box");

            string newName = EditorGUILayout.TextField("Name", item.Name);

            if (newName != oldKey && !string.IsNullOrEmpty(newName) && !_items.ContainsKey(newName))
            {
                renameList.Add((oldKey, newName));
                item.Name = newName;
            }
            else
            {
                item.Name = newName;
            }

            item.Description = EditorGUILayout.TextField("Description", item.Description);
            item.Rarity = (EItemRarity)EditorGUILayout.EnumPopup("Rarity", item.Rarity);
            item.ItemType = (EItemType)EditorGUILayout.EnumPopup("ItemType", item.ItemType);

            DrawStatDictionary("AdditiveStats", item.AdditiveStats);
            DrawStatDictionary("MultiplierStats", item.MultiplierStats);

            if (GUILayout.Button("❌ 삭제"))
            {
                toRemove.Add(oldKey);
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();

        // 삭제 처리
        foreach (var key in toRemove)
        {
            _items.Remove(key);
        }

        // 이름 변경 처리
        foreach (var (oldKey, newKey) in renameList)
        {
            if (_items.ContainsKey(oldKey))
            {
                var item = _items[oldKey];
                _items.Remove(oldKey);
                _items[newKey] = item;
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("➕ 새 아이템 추가"))
        {
            string newKey = "NewItem" + _items.Count;
            var newItem = new ItemData
            {
                Name = newKey,
                Description = "",
                Rarity = EItemRarity.Common,
                ItemType = EItemType.Weapon,
                AdditiveStats = new Dictionary<string, float>(),
                MultiplierStats = new Dictionary<string, float>()
            };
            _items[newKey] = newItem;
        }

        if (GUILayout.Button("💾 JSON 저장"))
        {
            SaveJson();
        }
    }

    private void DrawStatDictionary(string label, Dictionary<string, float> dict)
    {
        GUILayout.Label(label, EditorStyles.boldLabel);

        if (dict == null)
            dict = new Dictionary<string, float>();

        var allStats = (EStatType[])System.Enum.GetValues(typeof(EStatType));

        foreach (var stat in allStats)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(stat.ToString(), GUILayout.Width(150));

            // dict에 이미 값이 있으면 불러오고, 없으면 0으로 초기화
            float value = dict.TryGetValue(stat.ToString(), out var v) ? v : 0f;

            float newValue = EditorGUILayout.FloatField(value);

            if (newValue != value)
                dict[stat.ToString()] = newValue;

            // 삭제 버튼 대신 0으로 초기화할 수도 있음
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                if (dict.ContainsKey(stat.ToString()))
                    dict.Remove(stat.ToString());
            }

            EditorGUILayout.EndHorizontal();
        }
    }



    private void SaveJson()
    {
        string json = JsonConvert.SerializeObject(_items, Formatting.Indented);
        File.WriteAllText(_jsonPath, json);
        Debug.Log($"✅ JSON 저장 완료: {_jsonPath}");
        AssetDatabase.Refresh();
    }
}
#endif