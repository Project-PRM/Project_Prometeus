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
        SkillFactory.Register(ESkillName.BasicAttack.ToString(), () => new BasicAttack());
        SkillFactory.Register(ESkillName.DummySkill.ToString(), () => new DummySkill());
        SkillFactory.Register(ESkillName.DummyUltimate.ToString(), () => new DummyUltimate());
        SkillFactory.Register(ESkillName.DummyPassive.ToString(), () => new DummyPassive());
        SkillFactory.Register(ESkillName.AttackerPassive.ToString(), () => new AttackerPassive());
        SkillFactory.Register(ESkillName.AttackerSkill.ToString(), () => new AttackerSkill());
        SkillFactory.Register(ESkillName.AttackerUltimate.ToString(), () => new AttackerUltimate());
    }
}
