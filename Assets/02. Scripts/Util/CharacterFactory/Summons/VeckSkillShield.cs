using FOW;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class VeckSkillShield : MonoBehaviour, ISummonObject, IDamageable
{
    private CharacterBase _owner;
    private SkillData _data;
    private Renderer _renderer;

    private SkillData _ultimateData;

    private Color _emissionColor = new Color(191f / 255f, 24f / 255f, 0f) * 3f;

    // TODO : 스탯 modifier 영향 받게? 
    private float _maxHealth = 30f;
    private float _baseArmor = 20f;
    private float _curArmor = 20f;
    private float _curHealth;

    private float _timer = 0f;

    private string _myTeam;

    private bool _isInitialized = false;
    private bool _isUltimateState = false;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
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

    public void OnUltimateActivate(SkillData ultimateData)
    {
        // - 속성 1 : 방패 방어력 + 30 -> _curArmor에 30 더하기
        // -속성 2 : 방패 피격 시 전방 부채꼴(거리 2) 로 5 데미지
        // - 속성 3 : 2초 뒤 전방 부채꼴(거리 5 ) 로 20 데미지 및 넉백
        // 레이어 바꾸기 - 궁극기 레이어?
        /*shield.layer = LayerMask.NameToLayer("궁극기 실드 레이어 이름");*/
        //  코루틴 시작으로 구현하기?
        if (_isUltimateState) return;
        _ultimateData = ultimateData;

        _isUltimateState = true;
        _curArmor += 30f;

        PhotonView photonView = GetComponent<PhotonView>();
        photonView.RPC(nameof(RPC_SetEmission), RpcTarget.AllBuffered, true);

        StartCoroutine(UltimateAttackPhase());
    }

    public void OnUltimateEnd()
    {
        // 방패 방어력 원상복구
        // 방패 피격 시 대미지 끄기
        // 레이어 복구
        if (!_isUltimateState) return;

        PhotonView photonView = GetComponent<PhotonView>();
        photonView.RPC(nameof(RPC_SetEmission), RpcTarget.AllBuffered, false);

        _curArmor = _baseArmor;
        _isUltimateState = false;
    }

    private IEnumerator UltimateAttackPhase()
    {
        // 첫 피격 데미지는 RPC_TakeDamage 안에서 처리
        yield return new WaitForSeconds(2f);

        // 2초 뒤 강한 부채꼴 공격
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Collider[] targets = Physics.OverlapSphere(origin + direction * 3f, 2.5f); // 거리 5, 범위 조정

        foreach (var hit in targets)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.RPC_TakeDamage(_ultimateData.Damage);
                // 넉백도 원하면 적용
                Vector3 force = direction.normalized * 5f;
                if (hit.TryGetComponent<Rigidbody>(out var rb))
                    rb.AddForce(force, ForceMode.Impulse);
            }
        }

        // 종료 시 상태 복구
        OnUltimateEnd();
    }

    [PunRPC]
    public void RPC_SetEmission(bool enable)
    {
        if (_renderer == null) return;

        Material mat = _renderer.material;
        mat.SetColor("_EmissionColor", enable ? _emissionColor : Color.black);
    }

    public void RPC_TakeDamage(float damage)
    {
        _curHealth -= DamageCalculator.CalculateDamage(damage, _curArmor);

        if (_isUltimateState)
        {
            Vector3 origin = transform.position;
            Vector3 direction = transform.forward;

            Collider[] targets = Physics.OverlapSphere(origin + direction * 1.5f, 1.5f); // 거리 2

            foreach (var hit in targets)
            {
                if (hit.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.RPC_TakeDamage(_data.Damage); // TickDamage 또는 하드코딩
                }
            }
        }

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
