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

    [Header("# Components")]
    public Animator Animator { get; private set; }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        if (_playerInput == null)
        {
            Debug.LogError("PlayerInput component is missing on CharacterBehaviour.");
        }
    }

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

    private async void Start()
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
        _character.TakeDamage(Damage);
    }

    public void Heal(float Amount)
    {
        _character.Heal(Amount);
    }
}