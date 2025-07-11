public class CharacterBase
{
    public string Name { get; private set; }
    public CharacterBehaviour Behaviour { get; private set; }

    public CharacterStats BaseStats { get; private set; } // Firebase 기반
    public CharacterStats FinalStats => StatCalculator.CalculateFinalStats(BaseStats, Equipment);

    public EquipmentSet Equipment { get; private set; }

    private ISkill _basicAttack;
    private ISkill _passive;
    private ISkill _skill;
    private ISkill _ultimate;

    public CharacterBase(CharacterBehaviour behaviour, string name, ISkill basicAttack, ISkill passive, ISkill skill, ISkill ultimate, CharacterStats baseStats)
    {
        Behaviour = behaviour;
        Name = name;
        _basicAttack = basicAttack;
        _passive = passive;
        _skill = skill;
        _ultimate = ultimate;
        BaseStats = baseStats;
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

    public void Update()
    {
        _basicAttack.Update();
        _passive.Update();
        _skill.Update();
        _ultimate.Update();
    }
}
