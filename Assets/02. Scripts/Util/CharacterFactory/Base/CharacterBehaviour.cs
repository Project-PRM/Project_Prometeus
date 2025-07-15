using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterBehaviour : /*PlayerActivity,*/MonoBehaviour, IStatusAffectable
{
    [SerializeField] private ECharacterName _characterName;

    private CharacterBase _character;
    public CharacterBase GetCharacterBase() => _character;

    private bool _isInitialized = false;

    private List<IStatusEffect> _activeEffects = new();

    [Header("# Skill Use State")]
    private ESkillInputState _inputState = ESkillInputState.None;
    private ESkillType? _selectedSkill = null;
    private Vector3 _aimDirection = Vector3.zero;
    private enum ESkillInputState
    {
        None,
        Aiming
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

    /*public void OnAttack(InputAction.CallbackContext callback)
    {
        if (!_photonView.IsMine) return;

        if (callback.performed)
        {
            // 일반 공격을 함
            Debug.Log("Normal Attack performed");
            _character.UseSkill(ESkillType.BasicAttack);
            _photonView.RPC(nameof(RPC_NormalAttack), RpcTarget.All);
        }
    }*/

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
            case ESkillInputState.None:
                HandleSkillKeyInput();
                break;

            case ESkillInputState.Aiming:
                UpdateAimingUI();
                HandleAimingInput();
                break;
        }

        if (_inputState == ESkillInputState.None && Input.GetMouseButtonDown(0))
        {
            _character.UseSkill(ESkillType.BasicAttack);
        }
    }

    private void HandleSkillKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryActivateSkillOrEnterAiming(ESkillType.Skill);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            TryActivateSkillOrEnterAiming(ESkillType.Ultimate);
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            TryActivateSkillOrEnterAiming(ESkillType.Passive);
        }
    }

    private void TryActivateSkillOrEnterAiming(ESkillType skillType)
    {
        ISkill skill = _character.GetSkill(skillType);

        if (skill is ISkillNoTarget noTargetSkill)
        {
            noTargetSkill.Activate(_character); // 즉시 발동
        }
        else
        {
            EnterAimingMode(skillType); // 조준 모드 진입
        }
    }

    private void EnterAimingMode(ESkillType skillType)
    {
        _selectedSkill = skillType;
        _inputState = ESkillInputState.Aiming;
        //_aimIndicator.SetActive(true);
    }

    private void UpdateAimingUI()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        _aimDirection = (mouseWorld - transform.position).normalized;

        //_aimIndicator.transform.position = transform.position;
        //_aimIndicator.transform.rotation = Quaternion.LookRotation(Vector3.forward, _aimDirection);
    }

    public void ApplyEffect(IStatusEffect effect)
    {
        effect.Apply(_character);
        _activeEffects.Add(effect);
        StartCoroutine(RemoveEffectAfterTime(effect));
    }

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

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // y = 0인 지면과의 충돌점을 구함
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return ray.GetPoint(10f); // 실패 시 fallback
    }

    private CharacterBase GetTargetUnderMouse(Vector3 worldPoint)
    {
        Collider[] hits = Physics.OverlapSphere(worldPoint, 0.5f); // 반경은 필요에 따라 조정
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<CharacterBehaviour>(out var behaviour))
            {
                return behaviour.GetCharacterBase();
            }
        }
        return null;
    }

    private void ResetSkillState()
    {
        _selectedSkill = null;
        _inputState = ESkillInputState.None;
        //_aimIndicator.SetActive(false);
    }

    public void RemoveEffect(IStatusEffect effect)
    {
        effect.Remove(_character);
        _activeEffects.Remove(effect);
    }

    private IEnumerator RemoveEffectAfterTime(IStatusEffect effect)
    {
        yield return new WaitForSeconds(effect.Duration);
        RemoveEffect(effect);
    }
}
