using Photon.Pun;
using UnityEngine;

public class VeckSkill : ISkillNoTarget
{
    private float _timer = 0f;
    public SkillData Data { get; set; }
    public CharacterBase Character { get; set; }

    private VeckSkillShield _currentShield;

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

        // 기존 방패 제거
        if (_currentShield != null)
        {
            PhotonNetwork.Destroy(_currentShield.gameObject);
        }

        Debug.Log($"{Character.Name} activated VeckSkill.");

        // 방패 프리팹 로드
        GameObject prefab = Resources.Load<GameObject>("Summons/" + Data.SummonPrefabName);
        if (prefab == null)
        {
            Debug.LogError($"Shield prefab '{Data.SummonPrefabName}' not found in Resources/Summons.");
            return;
        }

        Vector3 spawnPos = Character.Behaviour.transform.position + Character.Behaviour.transform.forward * 1.2f;
        GameObject shieldObj = PhotonNetwork.Instantiate($"Summons/{Data.SummonPrefabName}", spawnPos, Quaternion.identity);
        _currentShield = shieldObj.GetComponent<VeckSkillShield>();

        if (_currentShield == null)
        {
            Debug.LogError("VeckSkillSummon component missing from spawned object.");
            return;
        }

        _currentShield.SetData(Data, Character);

        // 이동속도 감소

        _timer = 0f;
    }
}
