using UnityEngine;

[Skill("DummyPassive")]
public class DummyPassive : IEventReactiveSkill
{
    private float _timer = 0f;

    public SkillData Data { get; set; }

    public void Update()
    {
        _timer += Time.deltaTime;
    }

    public void Activate(CharacterBase character)
    {
        Debug.Log($"{character.Name} activated DummyPassive.");
    }

    public void OnEvent(ECharacterEvent evt)
    {
        if(evt != ECharacterEvent.OnBasicAttack)
        {
            return;
        }

        Debug.Log($"패시브 추뎀 발동 : {Data.Damage}");
    }
}