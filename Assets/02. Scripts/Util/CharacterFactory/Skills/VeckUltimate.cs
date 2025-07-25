using UnityEngine;

public class VeckUltimate : ISkillNoTarget
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
            Debug.Log($"{Character.Name} Skill is on cooldown.");
            return;
        }

        Debug.Log($"{Character.Name} activated VeckUltimate.");

        CharacterController controller = Character.Behaviour.GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogWarning("CharacterController not found on character.");
            return;
        }


        _timer = 0f;
    }
}
