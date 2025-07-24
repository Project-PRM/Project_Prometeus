using UnityEngine;
using System.Collections.Generic;

public class BuffUltimate : ISkillNoTarget
{
    private float _timer = 0f;

    public SkillData Data { get; set; }
    public CharacterBase Character { get; set; }
    public void SetOwner(CharacterBase character)
    {
        Character = character;
    }

    public void Update()
    {
        _timer += Time.deltaTime;
    }

    public void Activate()
    {
        if(_timer < Data.Cooltime)
        {
            Debug.Log($"{Character.Name} Ultimate is on cooldown.");
            return;
        }

        Vector3 center = Character.Behaviour.transform.position;
        float radius = Data.Radius;

        Collider[] hits = Physics.OverlapSphere(center, radius);
        List<CharacterBase> targets = new List<CharacterBase>();

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<CharacterBehaviour>(out var behaviour))
            {
                // TODO : 팀원만 타겟팅 (같은 팀 여부 판단 로직 필요)
                CharacterBase targetCharacter = behaviour.GetCharacterBase();

                if (targetCharacter.Team != Character.Team) continue;

                if (targetCharacter.Behaviour.TryGetComponent<IStatusAffectable>(out var statusAffectable))
                {
                    statusAffectable.ApplyEffect(new ArmorBuffEffect(Data.BuffAmount, Data.Duration));
                }
            }
        }

        Debug.Log($"Found {targets.Count} characters in radius {radius}.");

        _timer = 0f;
    }
}
