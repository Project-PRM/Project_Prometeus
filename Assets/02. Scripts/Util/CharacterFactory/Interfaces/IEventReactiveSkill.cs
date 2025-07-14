public interface IEventReactiveSkill : ISkill
{
    public void OnEvent(ECharacterEvent evt);
}
