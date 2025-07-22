using UnityEngine;

public class SpawnerPassive : IPermanentSkill
{
    public SkillData Data { get; set; }

    public void Update()
    {

    }

    public GameObject GetIndicatorPrefab()
    {
        return Resources.Load<GameObject>($"Indicators/{Data.IndicatorPrefabName}");
    }

    public void Activate(CharacterBase character)
    {
        Debug.Log($"{character.Name} activated SpawnerPassive.");

        CharacterController controller = character.Behaviour.GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogWarning("CharacterController not found on character.");
            return;
        }
    }
}
