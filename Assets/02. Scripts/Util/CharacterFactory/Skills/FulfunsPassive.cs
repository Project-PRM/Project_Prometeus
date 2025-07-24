using UnityEngine;
using Photon.Pun;
using static UnityEditor.PlayerSettings;
using System.Collections;

public class FulfunsPassive : IEventReactiveSkill
{
    private float _timer = 0f;

    private bool _isActive = false;
    private float _activeTimeRemaining = 0f;
    private float _spawnCooldownTimer = 0f;

    private Coroutine _smallPuddles;

    public SkillData Data { get; set; }

    public CharacterBase Character { get; set; }
    public void SetOwner(CharacterBase character)
    {
        Character = character;
    }

    public void Update()
    {
        _timer += Time.deltaTime;

        Debug.Log("is it active?");
        if (!_isActive) return;
        Debug.Log("Yes it is");

        _activeTimeRemaining -= Time.deltaTime;
        _spawnCooldownTimer -= Time.deltaTime;

        if (_activeTimeRemaining <= 0f)
        {
            _isActive = false;
            return;
        }

        if (_spawnCooldownTimer <= 0f)
        {
            Vector3 spawnPos = Character.Behaviour.transform.position;
            spawnPos.y = 0.1f; // 땅에 살짝 띄워서 생성
            Debug.Log("Trying to instantiate AoE at " + spawnPos);
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
        Character = character;

        if(_smallPuddles == null)
        {
            _smallPuddles = Character.Behaviour.StartCoroutine(SpawnSmallPuddles());

        }
        _isActive = true;
        _activeTimeRemaining = 5f;
        _spawnCooldownTimer = 0f;

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
        puddles.StartAoEField(Character, Data.Duration, Data.Damage);
    }

    private IEnumerator SpawnSmallPuddles()
    {
        float duration = 5f;
        float tick = 1f;
        var tickSec = new WaitForSeconds(tick);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 spawnPos = Character.Behaviour.transform.position;
            spawnPos.y = 0.1f;

            Debug.Log("Spawn puddle at " + spawnPos);
            SpawnSmallAoE(spawnPos);

            yield return tickSec;
            elapsed += tick;
        }

        // 쿨타임용
        yield return new WaitForSeconds(6f);

        _smallPuddles = null;
    }
}
