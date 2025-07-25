using FOW;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class VeckSkillShield : MonoBehaviour, ISummonObject, IDamageable
{
    private CharacterBase _owner;
    private SkillData _data;

    // TODO : 스탯 modifier 영향 받게? 
    private float _maxHealth = 30f;
    private float _baseArmor = 20f;
    private float _curArmor = 20f;
    private float _curHealth;

    private float _timer = 0f;

    private string _myTeam;

    private bool _isInitialized = false;
    private bool _isUltimateState = false;

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

        // 위치 움직임
        Vector3 offset = _owner.Behaviour.transform.forward * 1.3f;
        transform.position = _owner.Behaviour.transform.position + offset + Vector3.up * 1.6f;

        // 각도 회전
        Quaternion rotation = Quaternion.LookRotation(_owner.Behaviour.transform.forward, Vector3.up);
        Vector3 euler = rotation.eulerAngles;
        euler.y += 180f;
        euler.z += 25f;
        transform.rotation = Quaternion.Euler(euler);
    }

    public void OnUltimateActivate()
    {
        // - 속성 1 : 방패 방어력 + 30 -> _curArmor에 30 더하기
        // -속성 2 : 방패 피격 시 전방 부채꼴(거리 2) 로 5 데미지
        // - 속성 3 : 2초 뒤 전방 부채꼴(거리 5 ) 로 20 데미지 및 넉백
        // 레이어 복구
        //  코루틴 시작으로 구현하기?
        if (_isUltimateState) return;

        _isUltimateState = true;
        _curArmor += 30f;

        StartCoroutine(UltimateAttackPhase());
    }

    public void OnUltimateEnd()
    {
        // 방패 방어력 원상복구
        // 방패 피격 시 대미지 끄기
        // 레이어 복구
        if (!_isUltimateState) return;

        _curArmor = _baseArmor;
        _isUltimateState = false;
    }

    private IEnumerator UltimateAttackPhase()
    {
        float delayBeforeSecondHit = 2f;

        // 첫 피격 데미지는 RPC_TakeDamage 안에서 처리
        yield return new WaitForSeconds(delayBeforeSecondHit);

        // 2초 뒤 강한 부채꼴 공격
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Collider[] targets = Physics.OverlapSphere(origin + direction * 3f, 2.5f); // 거리 5, 범위 조정

        foreach (var hit in targets)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.RPC_TakeDamage(20f);
                // 넉백도 원하면 적용
                Vector3 force = direction.normalized * 5f;
                if (hit.TryGetComponent<Rigidbody>(out var rb))
                    rb.AddForce(force, ForceMode.Impulse);
            }
        }

        // 종료 시 상태 복구
        OnUltimateEnd();
    }

    public void RPC_TakeDamage(float damage)
    {
        // 현재 방어 수치 애매
        _curHealth -= DamageCalculator.CalculateDamage(damage, _curArmor);

        // 궁극기 상태일 경우 : 

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
        _curArmor = _baseArmor;

        _isInitialized = true;
    }
}
