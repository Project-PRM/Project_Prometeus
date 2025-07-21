using System.Collections;
using UnityEngine;

/// <summary>
/// 자기 자신 대쉬
/// </summary>
public class AttackerPassive : ISkillNoTarget
{
    private float _timer = 0f;
    public SkillData Data { get; set; }
    private Coroutine _dash;

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
        if(_timer < Data.Cooltime)
        {
            Debug.Log($"{character.Name} Passive is on cooldown.");
            return;
        }

        Debug.Log($"{character.Name} activated AttackerPassive.");

        CharacterController controller = character.Behaviour.GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogWarning("CharacterController not found on character.");
            return;
        }

        /*// 대시 거리와 방향 설정
        float dashDistance = 5f;
        Vector3 dashDirection = character.Behaviour.transform.forward; // 또는 원하는 방향

        controller.Move(dashDirection.normalized * dashDistance);*/

        // 대시 설정
        Vector3 dashDirection = character.Behaviour.transform.forward;

        // 이전 대시가 있으면 중단
        if (_dash != null)
        {
            return;
        }

        // 새 대시 실행
        _dash = character.Behaviour.StartCoroutine(AttackerPassiveDash(controller, dashDirection, Data.Duration, Data.Speed));

        _timer = 0f;
    }

    private IEnumerator AttackerPassiveDash(CharacterController controller, Vector3 direction, float duration, float speed)
    {
        float elapsed = 0f;
        Debug.Log($"Used {Data.SkillName}");
        while (elapsed < duration)
        {
            controller.Move(direction.normalized * speed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _dash = null;
    }
}
