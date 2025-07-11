using System;
using System.Linq;
using System.Reflection;

public static class SkillRegistrar
{
    // 완전 자동화 보류
    /*public static void RegisterAll()
    {
        var skillTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => typeof(ISkill).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in skillTypes)
        {
            var attr = type.GetCustomAttribute<SkillAttribute>();
            if (attr != null)
            {
                SkillFactory.Register(attr.SkillName, () => (ISkill)Activator.CreateInstance(type));
            }
        }
    }*/

    public static void RegisterAll()
    {
        // 스킬 만들 때 여기에 등록
        SkillFactory.Register("BasicAttack", () => new BasicAttack());
        SkillFactory.Register("DummySkill", () => new DummySkill());
        SkillFactory.Register("DummyUltimate", () => new DummyUltimate());
        SkillFactory.Register("DumyPassive", () => new DummyPassive());
    }
}
