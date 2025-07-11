public interface ISkill
{
    public void Update();
    public SkillData Data { get; set; }
    public void Activate(CharacterBase user);
}