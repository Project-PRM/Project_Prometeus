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
    private GameObject _aimIndicator;
    private enum ESkillInputState
    {
        None,
        Aiming
    }

    // 캐릭터 스킬 조합
    //protected override async void Start()
    //{
    //    base.Start();
    //    if (!CharacterManager.Instance.IsInitialized) 
    //    {
    //        await CharacterManager.Instance.Init();
    //    }
    //    var skills = await CharacterManager.Instance.GetCharacterMetaDataAsync(_characterName);

    //    _character = new CharacterBase(
    //        this,
    //        _characterName.ToString(),
    //        SkillFactory.Create(ESkillType.BasicAttack.ToString()),
    //        SkillFactory.Create(skills.Passive),
    //        SkillFactory.Create(skills.Skill),
    //        SkillFactory.Create(skills.Ultimate),
    //        CharacterManager.Instance.CharacterStats
    //    );

    //    _isInitialized = true;
    //}

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

    //public void OnAttack(InputAction.CallbackContext callback)
    //{
    //    if (callback.performed)
    //    {
    //        // 일반 공격을 함
    //        Debug.Log("Normal Attack performed");
    //        _character.UseSkill(ESkillType.BasicAttack);
    //    }
    //}

    [PunRPC]
    private void RPC_NormalAttack()
    {
        Debug.Log("Normal Attack performed via RPC");
        _character.UseSkill(ESkillType.BasicAttack);
    }

    //private void Update()
    //{
    //    if(_isInitialized == false)
    //    {
    //        return;
    //    }
    //    _character.Update();

    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        _character.UseSkill(ESkillType.BasicAttack);
    //    }
    //    if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //        _character.UseSkill(ESkillType.Skill);
    //    }
    //    if (Input.GetKeyDown(KeyCode.Z))
    //    {
    //        _character.UseSkill(ESkillType.Ultimate);
    //    }
    //}

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

        // 평타는 그대로
        if (_inputState == ESkillInputState.None && Input.GetMouseButtonDown(0))
        {
            _character.UseSkill(ESkillType.BasicAttack);
        }
    }

    private void HandleSkillKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            EnterAimingMode(ESkillType.Skill);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            EnterAimingMode(ESkillType.Ultimate);
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
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                Vector3 targetPoint;
                CharacterBase targetUnit = null;

                if (Physics.Raycast(ray, out hit, 100f)) // 100f는 최대 거리
                {
                    targetPoint = hit.point;

                    // 대상 캐릭터가 있다면
                    if (hit.collider.TryGetComponent<CharacterBehaviour>(out var behaviour))
                    {
                        targetUnit = behaviour.GetCharacterBase(); // 이건 CharacterBehaviour에 따로 만들어야 함
                    }
                }
                else
                {
                    // 바닥 등 맞은 게 없을 경우, 카메라 앞 10m 지점으로 임시 설정
                    targetPoint = ray.GetPoint(10f);
                }

                _character.UseSkill(_selectedSkill.Value, targetUnit, targetPoint);
            }

            ResetSkillState();
        }

        if (Input.GetMouseButtonDown(1)) // 우클릭: 취소
        {
            ResetSkillState();
        }
    }


    private CharacterBase GetTargetUnderMouse(Vector3 mouseWorld)
    {
        Collider[] hits = Physics.OverlapSphere(mouseWorld, 0.2f); // 또는 Physics.Raycast
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
