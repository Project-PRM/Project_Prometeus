using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterStatusEffect))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(CharacterInputHandler))]
[RequireComponent(typeof(Animator))]
public class CharacterBehaviour : MonoBehaviour, IDamageable
{
    [SerializeField] private ECharacterName _characterName;

    private CharacterBase _character;
    public CharacterBase GetCharacterBase() => _character;
    private CharacterAimingController _aimingController;

    private bool _isInitialized = false;

    [Header("# Components")]
    public Animator Animator { get; private set; }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        _aimingController = new CharacterAimingController(this);
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
        if (_aimingController.IsAiming)
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

    private void Update()
    {
        if (!_isInitialized) return;

        _character.Update();
        _aimingController.Update();
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
            _aimingController.EnterAimingMode(skillType);
        }
    }

    // 마우스 위치에 있는 캐릭터를 찾아 반환 (없으면 null)
    public CharacterBase GetTargetUnderMouse(Vector3 worldPoint)
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

    public void TakeDamage(float Damage)
    {
        _character.TakeDamage(Damage);
    }

    public void Heal(float Amount)
    {
        _character.Heal(Amount);
    }
}