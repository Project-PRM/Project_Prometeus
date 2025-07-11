using UnityEngine;

[Skill("DummyPassive")]
public class DummyPassive : ISkill
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
}