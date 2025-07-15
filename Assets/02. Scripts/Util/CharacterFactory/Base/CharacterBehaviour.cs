using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterStatusEffect))]
[RequireComponent(typeof(PhotonView))]
public class CharacterBehaviour : MonoBehaviour, IDamageable
{
    [SerializeField] private ECharacterName _characterName;
    [SerializeField] private PlayerInput _playerInput; // PlayerInput 컴포넌트 연결 필요

    private CharacterBase _character;
    public CharacterBase GetCharacterBase() => _character;

    private bool _isInitialized = false;

    [Header("# Status Effects")]
    private List<StatusEffectInstance> _activeEffects = new();

    [Header("# Skill Use State")]
    private ESkillInputState _inputState = ESkillInputState.None;
    private ESkillType? _selectedSkill = null;
    private Vector3 _aimDirection = Vector3.zero;

    private void OnEnable()
    {
        _playerInput.actions["Passive"].performed += OnPassiveUse;
        _playerInput.actions["Skill"].performed += OnSkillUse;
        _playerInput.actions["Ultimate"].performed += OnUltimateUse;
        _playerInput.actions["Attack"].performed += OnAttack;
    }

    private void OnDisable()
    {
        _playerInput.actions["Passive"].performed -= OnPassiveUse;
        _playerInput.actions["Skill"].performed -= OnSkillUse;
        _playerInput.actions["Ultimate"].performed -= OnUltimateUse;
        _playerInput.actions["Attack"].performed -= OnAttack;
    }

    protected async void Start()
    {
        if (!CharacterManager.Instance.IsInitialized)
        {
            await CharacterManager.Instance.Init();
        }
        var skills = await CharacterManager.Instance.GetCharacterMetaDataAsync(_characterName);

        _character = new CharacterBase(
            this,
            _characterName.ToString(),
            SkillFactory.Create(ESkillType.BasicAttack.ToString()),
            SkillFactory.Create(skills.Passive),
            SkillFactory.Create(skills.Skill),
            SkillFactory.Create(skills.Ultimate),
            CharacterManager.Instance.CharacterStats
        );

        _isInitialized = true;
    }

    public void OnAttack(InputAction.CallbackContext callback)
    {
        //if (!_photonView.IsMine) return;
        if (_inputState == ESkillInputState.Aiming)
            return;
        TryActivateSkillOrEnterAiming(ESkillType.BasicAttack);
    }

    public void OnSkillUse(InputAction.CallbackContext callback)
    {
        //if (!_photonView.IsMine) return;
        TryActivateSkillOrEnterAiming(ESkillType.Skill);
    }

    public void OnUltimateUse(InputAction.CallbackContext callback)
    {
        //if (!_photonView.IsMine) return;
        TryActivateSkillOrEnterAiming(ESkillType.Ultimate);
    }

    public void OnPassiveUse(InputAction.CallbackContext callback)
    {
        //if (!_photonView.IsMine) return;
        TryActivateSkillOrEnterAiming(ESkillType.Passive);
    }

    [PunRPC]
    private void RPC_NormalAttack()
    {
        Debug.Log("Normal Attack performed via RPC");
        _character.UseSkill(ESkillType.BasicAttack);
    }

    private void Update()
    {
        if (!_isInitialized) return;

        _character.Update();

        switch (_inputState)
        {
            case ESkillInputState.Aiming:
                UpdateAimingUI();
                HandleAimingInput();
                break;
            default:
                break;
        }
    }

    // 스킬 타입에 따라 즉발 스킬은 즉시 실행, 아니면 조준 모드로 진입
    private void TryActivateSkillOrEnterAiming(ESkillType skillType)
    {
        ISkill skill = _character.GetSkill(skillType);

        if(skill is IPermanentSkill permanentSkill)
        {
            return;
        }
        else if (skill is ISkillNoTarget noTargetSkill)
        {
            noTargetSkill.Activate(_character); // 즉시 발동
        }
        else
        {
            EnterAimingMode(skillType); // 조준 모드 진입
        }
    }

    // 조준 모드 설정
    private void EnterAimingMode(ESkillType skillType)
    {
        _selectedSkill = skillType;
        _inputState = ESkillInputState.Aiming;
        //_aimIndicator.SetActive(true);
    }

    // 조준 UI 업데이트 (에임 방향 벡터 갱신 등)
    private void UpdateAimingUI()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        _aimDirection = (mouseWorld - transform.position).normalized;

        //_aimIndicator.transform.position = transform.position;
        //_aimIndicator.transform.rotation = Quaternion.LookRotation(Vector3.forward, _aimDirection);
    }

    // 조준 상태에서 마우스 클릭 시 스킬 시전
    private void HandleAimingInput()
    {
        if (Input.GetMouseButtonDown(0)) // 좌클릭: 시전
        {
            if (_selectedSkill.HasValue)
            {
                Vector3 targetPoint = GetMouseWorldPosition();
                CharacterBase targetUnit = GetTargetUnderMouse(targetPoint);

                _character.UseSkill(_selectedSkill.Value, targetUnit, targetPoint);
            }

            ResetSkillState();
        }

        if (Input.GetMouseButtonDown(1)) // 우클릭: 취소
        {
            ResetSkillState();
        }
    }

    // 마우스 클릭 위치의 월드 좌표 반환 (y = 0 평면 기준)
    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return ray.GetPoint(10f); // 실패 시 fallback
    }

    // 마우스 위치에 있는 캐릭터를 찾아 반환 (없으면 null)
    private CharacterBase GetTargetUnderMouse(Vector3 worldPoint)
    {
        Collider[] hits = Physics.OverlapSphere(worldPoint, 0.5f);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<CharacterBehaviour>(out var behaviour))
            {
                return behaviour.GetCharacterBase();
            }
        }
        return null;
    }

    // 조준 상태 및 선택된 스킬 초기화
    private void ResetSkillState()
    {
        _selectedSkill = null;
        _inputState = ESkillInputState.None;
        //_aimIndicator.SetActive(false);
    }

    public void TakeDamage(float Damage)
    {
        //throw new System.NotImplementedException();
    }

    public void Heal(float Amount)
    {
        //throw new System.NotImplementedException();
    }

    /*public void ApplyEffect(IStatusEffect newEffect)
    {
        newEffect.Apply(_character);

        var coroutine = StartCoroutine(EffectDurationCoroutine(newEffect));
        var instance = new StatusEffectInstance(newEffect, coroutine, newEffect.Duration);

        _activeEffects.Add(instance);
    }

    public void RemoveEffect(IStatusEffect effect)
    {
        // 첫 번째로 매칭되는 인스턴스를 찾음(정확히 동일한 참조)
        var instance = _activeEffects.Find(e => e.Effect == effect);
        if (instance == null) return;

        // 코루틴 정지
        if (instance.Coroutine != null)
            StopCoroutine(instance.Coroutine);

        // 효과 제거 및 목록에서 제거
        effect.Remove(_character);
        _activeEffects.Remove(instance);
    }

    // 특정 타입의 효과를 모두 제거
    public void RemoveAllEffectsOfType<T>() where T : IStatusEffect
    {
        var toRemove = _activeEffects.FindAll(e => e.Effect is T);

        foreach (var instance in toRemove)
        {
            if (instance.Coroutine != null)
                StopCoroutine(instance.Coroutine);

            instance.Effect.Remove(_character);
            _activeEffects.Remove(instance);
        }
    }

    private IEnumerator EffectDurationCoroutine(IStatusEffect effect)
    {
        float remaining = effect.Duration;

        while (remaining > 0f)
        {
            remaining -= Time.deltaTime;
            yield return null;
        }

        effect.Remove(_character);

        // 리스트에서 해당 인스턴스를 제거
        _activeEffects.RemoveAll(e => e.Effect == effect);
    }*/
}