/// <summary>
/// 특정 상황에 반응해 발동하는 스킬 -> 직접호출 안하고 이벤트 기반으로 사용할 것
/// ex) 기본 공격시 추가 공격, 피격시 일정량 회복 등
/// </summary>
public interface IEventReactiveSkill : ISkill
{
    public void Activate();
    public void OnEvent(ECharacterEvent evt);
}
