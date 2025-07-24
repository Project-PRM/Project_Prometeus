using UnityEngine;
using Photon.Pun;
using Unity.AppUI.Core;
using UnityEngine.TextCore.Text;

public class FulfunsPassive : IEventReactiveSkill
{
    private float _timer = 0f;
    private CharacterBase _character;

    private bool _isActive = false;
    private float _activeTimeRemaining = 0f;
    private float _spawnCooldownTimer = 0f;

    public SkillData Data { get; set; }

    public void Update()
    {
        _timer += Time.deltaTime;

        if (!_isActive) return;

        _activeTimeRemaining -= Time.deltaTime;
        _spawnCooldownTimer -= Time.deltaTime;

        if (_activeTimeRemaining <= 0f)
        {
            _isActive = false;
            return;
        }

        if (_spawnCooldownTimer <= 0f)
        {
            Vector3 spawnPos = _character.Behaviour.transform.position;
            spawnPos.y = 0.1f; // 땅에 살짝 띄워서 생성
            SpawnSmallAoE(spawnPos);
            _spawnCooldownTimer = 1f; // 1초마다 생성
        }
    }

    public GameObject GetIndicatorPrefab()
    {
        return Resources.Load<GameObject>($"Indicators/{Data.IndicatorPrefabName}");
    }

    public void Activate(CharacterBase character)
    {
        _character = character;

        if (_timer < Data.Cooltime)
        {
            Debug.Log($"{character.Name} FulfunsPassive is on cooldown.");
            return;
        }
        Debug.Log($"{character.Name} activated FulfunsPassive.");

        _timer = 0f;
    }

    public void OnEvent(ECharacterEvent evt)
    {
        if (evt != ECharacterEvent.OnFulfunsFieldTouched)
        {
            return;
        }

        Debug.Log($"fulfuns passive start");

        _isActive = true;
        _activeTimeRemaining = 5f;
        _spawnCooldownTimer = 0f;
    }

    private void SpawnSmallAoE(Vector3 pos)
    {
        GameObject prefab = Resources.Load<GameObject>("Summons/" + Data.SummonPrefabName);

        if (prefab == null)
        {
            Debug.LogError($"프리팹 {Data.SummonPrefabName} 을(를) Resources/Summons 에서 찾을 수 없습니다.");
            return;
        }

        GameObject area = /*GameObject.*/PhotonNetwork.Instantiate($"Summons/{Data.SummonPrefabName}", pos, Quaternion.identity);
        AttackerAoEField puddles = area.GetComponent<AttackerAoEField>();
        puddles.StartAoEField(_character, Data.Duration, Data.Damage);
    }
}
