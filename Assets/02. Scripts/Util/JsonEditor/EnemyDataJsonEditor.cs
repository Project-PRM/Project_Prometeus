#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class EnemyDataJsonEditorWindow : EditorWindow
{
    private Dictionary<string, EnemyData> _enemies = new();
    private string _jsonPath = "Assets/Resources/Datas/enemy_data.json";
    private Vector2 _scrollPos;

    [MenuItem("Tools/Enemy/EnemyData JSON Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<EnemyDataJsonEditorWindow>("EnemyData JSON Editor");
        window.LoadJson();
    }

    private void LoadJson()
    {
        if (File.Exists(_jsonPath))
        {
            string json = File.ReadAllText(_jsonPath);
            _enemies = JsonConvert.DeserializeObject<Dictionary<string, EnemyData>>(json);
            Debug.Log($"✅ JSON 불러오기 성공: {_jsonPath}");
        }
        else
        {
            Debug.LogWarning($"⚠️ JSON 파일을 찾을 수 없습니다. 새로 생성합니다: {_jsonPath}");
            _enemies = new Dictionary<string, EnemyData>();
            SaveJson();
        }
    }

    private void SaveJson()
    {
        string json = JsonConvert.SerializeObject(_enemies, Formatting.Indented);
        File.WriteAllText(_jsonPath, json);
        Debug.Log($"✅ JSON 저장 완료: {_jsonPath}");
        AssetDatabase.Refresh();
    }

    private void OnGUI()
    {
        if (_enemies == null || _enemies.Count == 0)
        {
            EditorGUILayout.HelpBox("적 데이터가 없습니다.", MessageType.Info);
        }

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        List<string> toRemove = new();
        List<(string oldKey, string newKey)> renameList = new();

        foreach (var oldKey in new List<string>(_enemies.Keys))
        {
            var enemy = _enemies[oldKey];

            EditorGUILayout.BeginVertical("box");

            string newName = EditorGUILayout.TextField("Name", enemy.Name);

            if (newName != oldKey && !string.IsNullOrEmpty(newName) && !_enemies.ContainsKey(newName))
            {
                renameList.Add((oldKey, newName));
                enemy.Name = newName;
            }
            else
            {
                enemy.Name = newName;
            }

            enemy.MaxHealth = EditorGUILayout.FloatField("Max Health", enemy.MaxHealth);
            enemy.Armor = EditorGUILayout.FloatField("Armor", enemy.Armor);
            enemy.Speed = EditorGUILayout.FloatField("Speed", enemy.Speed);
            enemy.Damage = EditorGUILayout.FloatField("Damage", enemy.Damage);
            enemy.AttackCoolTime = EditorGUILayout.FloatField("Attack Cool Time", enemy.AttackCoolTime);
            enemy.AttackRange = EditorGUILayout.FloatField("Attack Range", enemy.AttackRange);
            enemy.VisionRange = EditorGUILayout.FloatField("Vision Range", enemy.VisionRange);
            enemy.DetectionRange = EditorGUILayout.FloatField("Detection Range", enemy.DetectionRange);
            enemy.ManaReward = EditorGUILayout.FloatField("Mana Reward", enemy.ManaReward);

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Drop Item Rewards", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Item Name", GUILayout.Width(200));
            EditorGUILayout.LabelField("드롭 확률", GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();

            if (enemy.DropItemRewards == null)
                enemy.DropItemRewards = new Dictionary<string, float>();

            List<string> keysToRemove = new();

            foreach (var kvp in new Dictionary<string, float>(enemy.DropItemRewards))
            {
                string key = kvp.Key;
                float value = kvp.Value;

                EditorGUILayout.BeginHorizontal();

                string newKey = EditorGUILayout.TextField(key, GUILayout.Width(200));
                float newValue = EditorGUILayout.Slider(value, 0f, 1f, GUILayout.Width(150));

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    keysToRemove.Add(key);
                }
                else
                {
                    if (newKey != key)
                    {
                        enemy.DropItemRewards.Remove(key);
                        enemy.DropItemRewards[newKey] = newValue;
                    }
                    else
                    {
                        enemy.DropItemRewards[key] = newValue;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            foreach (var key in keysToRemove)
            {
                enemy.DropItemRewards.Remove(key);
            }

            if (GUILayout.Button("➕ 드랍 아이템 추가"))
            {
                string newKey = "Item" + enemy.DropItemRewards.Count;
                if (!enemy.DropItemRewards.ContainsKey(newKey))
                    enemy.DropItemRewards[newKey] = 0f;
            }

            EditorGUILayout.Space(5);

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
            _enemies.Remove(key);
        }

        // 이름 변경 처리
        foreach (var (oldKey, newKey) in renameList)
        {
            if (_enemies.ContainsKey(oldKey))
            {
                var data = _enemies[oldKey];
                _enemies.Remove(oldKey);
                _enemies[newKey] = data;
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("➕ 새 Enemy 추가"))
        {
            string newKey = "NewEnemy" + _enemies.Count;
            var newEnemy = new EnemyData
            {
                Name = newKey,
                MaxHealth = 100,
                Armor = 0,
                Speed = 1,
                Damage = 10,
                AttackCoolTime = 1,
                AttackRange = 1,
                VisionRange = 5,
                DetectionRange = 7,
                ManaReward = 10,
                DropItemRewards = new Dictionary<string, float>()
            };
            _enemies[newKey] = newEnemy;
        }

        if (GUILayout.Button("💾 JSON 저장"))
        {
            SaveJson();
        }
    }
}
#endif