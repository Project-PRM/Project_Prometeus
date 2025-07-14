using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SkillAttribute : Attribute
{
    public string SkillName { get; }

    public SkillAttribute(string skillName)
    {
        SkillName = skillName;
    }
}
