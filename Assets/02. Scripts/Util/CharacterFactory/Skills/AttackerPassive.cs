using UnityEngine;

/// <summary>
/// 자기 자신 대쉬
/// </summary>
public class AttackerPassive : ISkill
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

        // 대시 거리와 방향 설정
        float dashDistance = 5f;
        Vector3 dashDirection = character.Behaviour.transform.forward; // 또는 원하는 방향

        controller.Move(dashDirection.normalized * dashDistance);

        _timer = 0f;
    }
}
