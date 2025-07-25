using FOW;
using Photon.Pun;
using UnityEngine;

public class VeckSkillShield : MonoBehaviour, ISummonObject, IDamageable
{
    private CharacterBase _owner;
    private SkillData _data;

    // TODO : 스탯 modifier 영향 받게? 
    private float _maxHealth = 30f;
    private float _baseArmor = 20f;
    private float _curHealth;

    private float _timer = 0f;

    private string _myTeam;

    private bool _isInitialized = false;

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

    private void LateUpdate()
    {
        if (!_isInitialized || _owner == null || _owner.Behaviour == null) return;

        Vector3 offset = _owner.Behaviour.transform.forward * 1.5f;
        transform.position = _owner.Behaviour.transform.position + offset + Vector3.up * 1.6f;
        transform.rotation = Quaternion.LookRotation(_owner.Behaviour.transform.forward, Vector3.up);
    }

    public void RPC_TakeDamage(float damage)
    {
        // 현재 방어 수치 애매
        _curHealth -= DamageCalculator.CalculateDamage(damage, _baseArmor);
        if (_curHealth <= 0)
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

        _isInitialized = true;
    }
}
