using System.Collections;
using System.Collections.Generic;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.TextCore.Text;

[Skill("BasicAttack")]
public class BasicAttack : ISkillNoTarget
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

    public void Activate(CharacterBase user)
    {
        // BasicAttack 스킬의 동작을 구현합니다.
        // 예시로, 공격력과 범위 등을 설정할 수 있습니다.
        if (_timer < Data.Cooltime)
        {
            Debug.Log($"{user.Name} Skill is on cooldown.");
            return;
        }
        _timer = 0f;

        // collider 및 collider 관련 키기 - 일단은 기본 범위, 무기 생기면 변경 필요
        // 실제 게임 로직에 맞게 공격력, 범위 등을 적용하는 코드를 추가해야 합니다.

        // 애니메이션
        user.Behaviour.Animator.SetTrigger("BasicAttack");

        bool current = user.Behaviour.Animator.GetBool("IsFirstAttack");
        bool next = !current;

        DamageTrigger trigger = user.Behaviour.DamageTrigger;
        user.Behaviour.StartCoroutine(EnableTriggerTemporarily(trigger, 0.5f));

        user.Behaviour.Animator.SetBool("IsFirstAttack", next);

        Debug.Log($"{user.Name}이(가) 기본 공격을 사용했습니다!");
    }

    private IEnumerator EnableTriggerTemporarily(DamageTrigger trigger, float duration)
    {
        trigger.TurnOnOrOff(true);
        yield return new WaitForSeconds(duration);
        trigger.TurnOnOrOff(false);
    }
}