using System.Collections.Generic;
using System;

public static class SkillFactory
{
    private static Dictionary<string, Func<ISkill>> _skillMap;
    private static Dictionary<string, SkillData> _skillDataMap;

    public static void Register(string skillName, Func<ISkill> constructor)
    {
        if (!_skillMap.ContainsKey(skillName))
            _skillMap.Add(skillName, constructor);
    }

    public static void LoadSkillData(Dictionary<string, SkillData> skillDataMap)
    {
        _skillDataMap = skillDataMap;
    }

    public static ISkill Create(string skillName)
    {
        if (_skillMap == null)
        {
            _skillMap = new Dictionary<string, Func<ISkill>>();
            SkillRegistrar.RegisterAll();
        }

        if (_skillMap.TryGetValue(skillName, out var constructor))
        {
            var skill = constructor();

            // 여기서 SkillData 주입
            if (_skillDataMap.TryGetValue(skillName, out var data))
            {
                skill.Data = data;
            }
            else
            {
                UnityEngine.Debug.LogWarning($"SkillData 없음: {skillName}");
            }

            return skill;
        }
        return null;
    }
}