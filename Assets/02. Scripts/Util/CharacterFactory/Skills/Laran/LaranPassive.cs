using Photon.Pun;
using System.Collections;
using UnityEngine;

public class LaranPassive : IPermanentSkill
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

    public GameObject GetIndicatorPrefab()
    {
        return Resources.Load<GameObject>($"Indicators/{Data.IndicatorPrefabName}");
    }

    public void Activate()
    {
        if (_timer < Data.Cooltime)
        {
            Debug.Log($"{Character.Name} LaranPassive is on cooldown.");
            return;
        }

        // 자신의 화염에 받는 데미지 50% 감소

        Debug.Log($"{Character.Name} activated LaranPassive.");
        _timer = 0f;
    }

    public void OnEvent(ECharacterEvent evt)
    {
        if (evt != ECharacterEvent.OnBasicAttack)
        {
            return;
        }
        Activate();
        Debug.Log($"Laran passive start");
    }
}