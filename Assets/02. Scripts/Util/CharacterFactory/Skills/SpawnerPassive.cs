using UnityEngine;

public class SpawnerPassive : ISkillNoTarget
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
            Debug.Log($"{character.Name} Passive is on cooldown.");
            return;
        }

        Debug.Log($"{character.Name} activated SpawnerPassive.");

        CharacterController controller = character.Behaviour.GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogWarning("CharacterController not found on character.");
            return;
        }

        _timer = 0f;
    }
}
