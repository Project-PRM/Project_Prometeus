using UnityEngine;

public class SpawnerSkill : ITargetableSkill
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

    public void Activate(Vector3 target)
    {
        if (_timer < Data.Cooltime)
        {
            Debug.Log($"{Character.Name} Skill is on cooldown.");
            return;
        }

        Vector3 origin = Character.Behaviour.transform.position + Character.Behaviour.transform.forward * 1.5f + Vector3.up;
        target.y = 1.5f; // Y축 고정하여 2D 발사 느낌을 주기 위함
        Vector3 dir = (target - origin).normalized;

        // 방향을 바라보도록 회전 (3D 기준으로 z축 전방)
        Quaternion rotation = Quaternion.LookRotation(dir);

        GameObject prefab = Resources.Load<GameObject>("Summons/" + Data.SummonPrefabName);

        if (prefab == null)
        {
            Debug.LogError($"프리팹 {Data.SummonPrefabName} 을(를) Resources/Summons 에서 찾을 수 없습니다.");
            return;
        }

        GameObject summon = GameObject.Instantiate(prefab, origin, rotation);
        summon.GetComponent<ISummonObject>().SetData(Data, Character);

        _timer = 0f;
    }
}
