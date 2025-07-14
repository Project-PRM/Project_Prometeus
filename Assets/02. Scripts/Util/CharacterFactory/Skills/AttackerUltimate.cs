using UnityEngine;

/// <summary>
/// 투사체 발사, 지속 데미지
/// </summary>
public class AttackerUltimate : ITargetableSkill
{
    private float _timer = 0f;

    public SkillData Data { get; set; }

    public void Update()
    {
        _timer += Time.deltaTime;
    }

    public void Activate(CharacterBase character, Vector3 target)
    {
        if (_timer < Data.Cooltime)
        {
            Debug.Log($"{character.Name} Ultimate is on cooldown.");
            return;
        }

        _timer = 0f;
    }
}