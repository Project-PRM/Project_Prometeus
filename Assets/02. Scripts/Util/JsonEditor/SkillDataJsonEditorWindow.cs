#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Linq;

public class SkillDataJsonEditorWindow : EditorWindow
{
    private Dictionary<string, SkillData> _skills = new();
    private string _jsonPath = "Assets/Resources/Datas/skill_data.json";
    private Vector2 _scrollPos;

    [MenuItem("Tools/Skill/SkillData JSON Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<SkillDataJsonEditorWindow>("SkillData JSON Editor");
        window.LoadJson();
    }

    private void LoadJson()
    {
        if (File.Exists(_jsonPath))
        {
            string json = File.ReadAllText(_jsonPath);
            _skills = JsonConvert.DeserializeObject<Dictionary<string, SkillData>>(json);
            Debug.Log($"✅ JSON 불러오기 성공: {_jsonPath}");
        }
        else
        {
            Debug.LogWarning($"⚠️ JSON 파일을 찾을 수 없습니다. 새로 생성합니다: {_jsonPath}");
            _skills = new Dictionary<string, SkillData>();
            SaveJson();
        }
    }

    private void SaveJson()
    {
        string json = JsonConvert.SerializeObject(_skills, Formatting.Indented);
        File.WriteAllText(_jsonPath, json);
        Debug.Log($"✅ JSON 저장 완료: {_jsonPath}");
        AssetDatabase.Refresh();
    }

    private void OnGUI()
    {
        if (_skills == null || _skills.Count == 0)
        {
            EditorGUILayout.HelpBox("스킬 데이터가 없습니다.", MessageType.Info);
        }

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        List<string> toRemove = new();
        List<(string oldKey, string newKey)> renameList = new();

        foreach (var oldKey in new List<string>(_skills.Keys))
        {
            var skill = _skills[oldKey];

            EditorGUILayout.BeginVertical("box");

            string newName = EditorGUILayout.TextField("Skill Name", skill.SkillName);

            if (newName != oldKey && !string.IsNullOrEmpty(newName) && !_skills.ContainsKey(newName))
            {
                renameList.Add((oldKey, newName));
                skill.SkillName = newName;
            }
            else
            {
                skill.SkillName = newName;
            }

            skill.Damage = EditorGUILayout.FloatField("Damage", skill.Damage);
            skill.Cost = EditorGUILayout.FloatField("Cost", skill.Cost);
            skill.Cooltime = EditorGUILayout.FloatField("Cooltime", skill.Cooltime);
            skill.Duration = EditorGUILayout.FloatField("Duration", skill.Duration);
            skill.Radius = EditorGUILayout.FloatField("Radius", skill.Radius);
            skill.MaxRange = EditorGUILayout.FloatField("Max Range", skill.MaxRange);
            skill.Speed = EditorGUILayout.FloatField("Speed", skill.Speed);

            skill.ProjectilePrefabName = EditorGUILayout.TextField("Projectile Prefab", skill.ProjectilePrefabName);
            skill.SummonPrefabName = EditorGUILayout.TextField("Summon Prefab", skill.SummonPrefabName);
            skill.IndicatorPrefabName = EditorGUILayout.TextField("Indicator Prefab", skill.IndicatorPrefabName);

            skill.BuffAmount = DrawEnumDictionary("Buff Amount", skill.BuffAmount);
            skill.DebuffAmount = DrawEnumDictionary("Debuff Amount", skill.DebuffAmount);


            if (GUILayout.Button("❌ 삭제"))
            {
                toRemove.Add(oldKey);
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();

        foreach (var key in toRemove)
        {
            _skills.Remove(key);
        }

        foreach (var (oldKey, newKey) in renameList)
        {
            if (_skills.ContainsKey(oldKey))
            {
                var data = _skills[oldKey];
                _skills.Remove(oldKey);
                _skills[newKey] = data;
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("➕ 새 Skill 추가"))
        {
            string newKey = "NewSkill" + _skills.Count;
            var newSkill = new SkillData
            {
                SkillName = newKey,
                Damage = 0,
                Cost = 0,
                Cooltime = 1,
                Duration = 1,
                Radius = 0,
                MaxRange = 0,
                Speed = 0,
                ProjectilePrefabName = "",
                SummonPrefabName = "",
                IndicatorPrefabName = "",
                BuffAmount = new Dictionary<string, float>(),
                DebuffAmount = new Dictionary<string, float>()
            };
            _skills[newKey] = newSkill;
        }

        if (GUILayout.Button("💾 JSON 저장"))
        {
            SaveJson();
        }
    }

    private Dictionary<string, float> DrawEnumDictionary(string label, Dictionary<string, float> targetDict)
    {
        var tempDict = ConvertToEnumDict(targetDict);

        // 열릴 때 EStatType 전부 다 들어가도록 자동 추가
        foreach (EStatType stat in Enum.GetValues(typeof(EStatType)))
        {
            if (!tempDict.ContainsKey(stat))
                tempDict[stat] = 0f;
        }

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

        List<EStatType> toRemove = new();

        foreach (var kvp in tempDict.ToList())
        {
            EditorGUILayout.BeginHorizontal();

            float newValue = EditorGUILayout.FloatField(kvp.Key.ToString(), kvp.Value);

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                toRemove.Add(kvp.Key);
            }
            else
            {
                tempDict[kvp.Key] = newValue;
            }

            EditorGUILayout.EndHorizontal();
        }

        foreach (var stat in toRemove)
        {
            tempDict.Remove(stat);
        }

        return ConvertToStringDict(tempDict);
    }

    private Dictionary<EStatType, float> ConvertToEnumDict(Dictionary<string, float> original)
    {
        var result = new Dictionary<EStatType, float>();
        foreach (var kvp in original)
        {
            if (Enum.TryParse(kvp.Key, out EStatType stat))
            {
                result[stat] = kvp.Value;
            }
        }
        return result;
    }

    private Dictionary<string, float> ConvertToStringDict(Dictionary<EStatType, float> original)
    {
        var result = new Dictionary<string, float>();
        foreach (var kvp in original)
        {
            result[kvp.Key.ToString()] = kvp.Value;
        }
        return result;
    }
}
#endif