using UnityEngine;

public class FulfunsPassive : IPermanentSkill
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
        Debug.Log("Activated FulfunsPassive");
    }
}
