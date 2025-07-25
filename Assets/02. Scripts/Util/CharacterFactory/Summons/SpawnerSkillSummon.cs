using Photon.Pun;
using Photon.Pun.Demo.SlotRacer.Utils;
using UnityEngine;
using FOW;

public class SpawnerSkillSummon : MonoBehaviour, ISummonObject, IDamageable
{
    private CharacterBase _owner;
    private SkillData _data;

    // TODO : 스탯 modifier 영향 받게? 
    private float _maxHealth = 20f;
    private float _curHealth;

    private float _timer = 0f;

    private string _myTeam;

    private bool _isInitialized = false;

    public void Start()
    {
        SpawnSlowField();
    }

    public void Update()
    {
        if (!_isInitialized)
        {
            return;
        }

        _timer += Time.deltaTime;

        Vector3 pos = transform.position;
        transform.position = pos;
    }

    private void SpawnSlowField()
    {
        /*GameObject prefab = Resources.Load<GameObject>("SlowField");
        *//*PhotonNetwork.*//*
        GameObject field = Instantiate(
        prefab,
        transform.position,
        Quaternion.identity,
        transform
        );

        SlowField slowField = field.GetComponent<SlowField>();
        slowField.StartSlowField(_owner, _debuffAmount);*/

        Vector3 spawnPosition = transform.position;

        GameObject field = PhotonNetwork.Instantiate("SlowField", spawnPosition, Quaternion.identity);
        SlowField slowField = field.GetComponent<SlowField>();
        slowField.StartSlowField(_owner, _data);
    }

    public void RPC_TakeDamage(float damage)
    {
        // 현재 방어 수치 애매
        _curHealth -= DamageCalculator.CalculateDamage(damage, 0);
        if(_curHealth <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void Heal(float heal)
    {
        _curHealth += heal;
        _curHealth = Mathf.Min(_curHealth, _maxHealth);
    }

    public void SetData(SkillData data, CharacterBase character, CharacterBase target = null)
    {
        _data = data;
        _owner = character;
        _myTeam = character.Team;

        _curHealth = _maxHealth;

        TurnOnRevealer();
        _isInitialized = true;
    }

    private void TurnOnRevealer()
    {
        var revealer = GetComponent<FogOfWarRevealer3D>();
        if (revealer == null || _owner == null) return;

        // 팀에 전체? 소환 플레이어 혼자?
        var ownerTeam = PhotonServerManager.Instance.GetPlayerTeam(_owner.Behaviour.PhotonView.Owner);

        if (_myTeam == ownerTeam)
        {
            revealer.enabled = true;
        }
        else
        {
            revealer.enabled = false;
        }
    }
}
