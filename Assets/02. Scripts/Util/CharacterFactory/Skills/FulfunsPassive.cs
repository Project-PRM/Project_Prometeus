using UnityEngine;

public class FulfunsPassive : IEventReactiveSkill
{
    private CharacterBase _character;
    private bool _isTracking = false;
    private float _activeTime = 5f;
    private float _cooldown = 1f;
    private float _lastSpawnTime = -Mathf.Infinity;
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

    public void OnEvent(ECharacterEvent evt)
    {

    }
}
