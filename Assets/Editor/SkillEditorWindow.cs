using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class SkillJsonEditorWindow : EditorWindow
{
    private Dictionary<string, SkillData> _skills = new();
    private string _jsonPath = "Assets/Resources/skill_data.json"; // 고정 경로
    private Vector2 _scrollPos;

    [MenuItem("Tools/Skill JSON Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<SkillJsonEditorWindow>("Skill JSON Editor");
        window.LoadDefaultJson();
    }

    private void LoadDefaultJson()
    {
        if (File.Exists(_jsonPath))
        {
            string json = File.ReadAllText(_jsonPath);
            _skills = JsonConvert.DeserializeObject<Dictionary<string, SkillData>>(json);
            Debug.Log($"✅ JSON 불러오기 성공: {_jsonPath}");
        }
        else
        {
            Debug.LogWarning($"⚠️ JSON 파일을 찾을 수 없습니다: {_jsonPath}");
            _skills = new Dictionary<string, SkillData>();
        }
    }

    private void OnGUI()
    {
        if (_skills == null || _skills.Count == 0)
        {
            EditorGUILayout.HelpBox("스킬 데이터가 없습니다.", MessageType.Info);
        }

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        List<string> toRemove = new();

        foreach (var key in new List<string>(_skills.Keys))
        {
            var skill = _skills[key];

            EditorGUILayout.BeginVertical("box");
            skill.SkillName = EditorGUILayout.TextField("Skill Name", skill.SkillName);
            skill.Damage = EditorGUILayout.FloatField("Damage", skill.Damage);
            skill.BuffAmount = EditorGUILayout.FloatField("BuffAmount", skill.BuffAmount);
            skill.DebuffAmount = EditorGUILayout.FloatField("DebuffAmount", skill.DebuffAmount);
            skill.Cost = EditorGUILayout.FloatField("Cost", skill.Cost);
            skill.Cooltime = EditorGUILayout.FloatField("Cooltime", skill.Cooltime);
            skill.Duration = EditorGUILayout.FloatField("Duration", skill.Duration);
            skill.Radius = EditorGUILayout.FloatField("Radius", skill.Radius);
            skill.MaxRange = EditorGUILayout.FloatField("MaxRange", skill.MaxRange);
            skill.Speed = EditorGUILayout.FloatField("Speed", skill.Speed);
            skill.ProjectilePrefabName = EditorGUILayout.TextField("Projectile Prefab Name", skill.ProjectilePrefabName);
            skill.IndicatorPrefabName = EditorGUILayout.TextField("Indicator Prefab Name", skill.IndicatorPrefabName);

            if (GUILayout.Button("❌ 삭제"))
            {
                toRemove.Add(key);
            }
            EditorGUILayout.EndVertical();
        }

        foreach (var key in toRemove)
        {
            _skills.Remove(key);
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (GUILayout.Button("➕ 새 스킬 추가"))
        {
            string newKey = "NewSkill" + _skills.Count;
            _skills[newKey] = new SkillData { SkillName = newKey };
        }

        if (GUILayout.Button("💾 JSON 저장"))
        {
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
}