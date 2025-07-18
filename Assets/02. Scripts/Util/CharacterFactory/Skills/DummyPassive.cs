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

    public GameObject GetIndicatorPrefab()
    {
        return Resources.Load<GameObject>($"Indicators/{Data.IndicatorPrefabName}");
    }

    public void Activate(CharacterBase character)
    {
        if (_timer < Data.Cooltime)
        {
            Debug.Log($"{character.Name} DummyPassive is on cooldown.");
            return;
        }
        Debug.Log($"{character.Name} activated DummyPassive.");

        _timer = 0f;
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