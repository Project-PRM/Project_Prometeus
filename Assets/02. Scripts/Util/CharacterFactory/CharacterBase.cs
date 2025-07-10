public class CharacterBase
{
    public string Name { get; private set; }
    public CharacterBehaviour Behaviour { get; private set; }

    private ISkill _basicAttack;
    private ISkill _passive;
    private ISkill _skill;
    private ISkill _ultimate;

    public CharacterBase(CharacterBehaviour behaviour, string name, ISkill basicAttack, ISkill passive, ISkill skill, ISkill ultimate)
    {
        Behaviour = behaviour;
        Name = name;
        _basicAttack = basicAttack;
        _passive = passive;
        _skill = skill;
        _ultimate = ultimate;
    }

    public CharacterBase(CharacterBehaviour behaviour, CharacterMetaData data)
    {
        Name = data.Name;
        _basicAttack = SkillFactory.Create(data.Basic);
        _passive = SkillFactory.Create(data.Passive);
        _skill = SkillFactory.Create(data.Skill);
        _ultimate = SkillFactory.Create(data.Ultimate);
    }

    public void UseSkill(ESkillType type)
    {
        switch (type)
        {
            case ESkillType.BasicAttack:
                _basicAttack.Activate(this); break;
            case ESkillType.Passive:
                _passive.Activate(this); break;
            case ESkillType.Skill:
                _skill.Activate(this); break;
            case ESkillType.Ultimate:
                _ultimate.Activate(this); break;
        }
    }
}
