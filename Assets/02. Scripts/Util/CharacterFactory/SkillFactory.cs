using System.Collections.Generic;
using System;

public static class SkillFactory
{
    private static Dictionary<string, Func<ISkill>> _skillMap = new();

    public static void Register(string skillName, Func<ISkill> constructor)
    {
        if (!_skillMap.ContainsKey(skillName))
            _skillMap.Add(skillName, constructor);
    }

    public static ISkill Create(string skillName)
    {
        if (_skillMap.TryGetValue(skillName, out var constructor))
            return constructor(); // 각 캐릭터에 새로 만들어서 넣어주기 위함
        else
            return null;
    }
}