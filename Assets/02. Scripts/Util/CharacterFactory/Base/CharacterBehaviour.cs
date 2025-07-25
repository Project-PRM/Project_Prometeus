using HighlightPlus;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterStatusEffect))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(CharacterInputHandler))]
[RequireComponent(typeof(Animator))]
public class CharacterBehaviour : MonoBehaviourPunCallbacks, IDamageable, ISelectable
{
    [SerializeField] private ECharacterName _characterName;

    private CharacterBase _character;
    public CharacterBase GetCharacterBase() => _character;
    private CharacterAimingController _aimingController;
    private CharacterMove _characterMove;
    private CharacterInGameView _gameView;

    private bool _isInitialized = false;

    [Header("# Components")]
    public Animator Animator { get; private set; }
    public CharacterController Controller { get; private set; }
    public PhotonView PhotonView { get; private set; }
    public DamageTrigger DamageTrigger { get; private set; }
    public CharacterInventory Inventory { get; private set; }
    public HighlightEffect HighlightEffect { get; set; }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();
        PhotonView = GetComponent<PhotonView>();
        DamageTrigger = GetComponentInChildren<DamageTrigger>();
        Inventory = GetComponent<CharacterInventory>();

        _characterMove = GetComponent<CharacterMove>();
        _gameView = GetComponent<CharacterInGameView>();
        _aimingController = new CharacterAimingController(this);
        HighlightEffect = GetComponent<HighlightEffect>();
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
            PhotonServerManager.Instance.GetPlayerTeam(PhotonNetwork.LocalPlayer),
            SkillFactory.Create(ESkillType.BasicAttack.ToString()),
            SkillFactory.Create(skills.Passive),
            SkillFactory.Create(skills.Skill),
            SkillFactory.Create(skills.Ultimate),
            CharacterManager.Instance.CharacterStats
        );
        DamageTrigger.Owner = this;
        _character.OnEventOccurred += _gameView.OnTakenDamage;
        _isInitialized = true;
    }

    public void OnAttack(InputAction.CallbackContext callback)
    {
        if (!PhotonView.IsMine)
        {
            return;
        }
        if (_aimingController.IsAiming)
            return;
        TryActivateSkillOrEnterAiming(ESkillType.BasicAttack);
    }

    public void OnSkillUse(InputAction.CallbackContext callback)
    {
        if (!PhotonView.IsMine) return;
        TryActivateSkillOrEnterAiming(ESkillType.Skill);
    }

    public void OnUltimateUse(InputAction.CallbackContext callback)
    {
        if (!PhotonView.IsMine) return;
        TryActivateSkillOrEnterAiming(ESkillType.Ultimate);
    }

    public void OnPassiveUse(InputAction.CallbackContext callback)
    {
        if (!PhotonView.IsMine) return;
        TryActivateSkillOrEnterAiming(ESkillType.Passive);
    }

    // for debug
    public void OnTakeDamage(InputAction.CallbackContext callback)
    {
        if (!PhotonView.IsMine) return;
        _character.TakeDamage(10);
    }

    private void Update()
    {
        if (!_isInitialized) return;
        if (!PhotonView.IsMine)
        {
            return;
        }

        _character.Update();
        _aimingController.Update();
        _characterMove?.Tick();
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
            noTargetSkill.Activate(); // 즉시 발동
        }
        else
        {
            _aimingController.EnterAimingMode(skillType);
        }
    }

    [PunRPC]
    public void RPC_TakeDamage(float Damage)
    {
        _character.TakeDamage(Damage);
    }

    public void Heal(float Amount)
    {
        _character.Heal(Amount);
    }

    public void AddStatModifier(StatModifier mod)
    {
        _character.AddStatModifier(mod);
    }

    public void RemoveStatModifier(StatModifier mod)
    {
        _character.RemoveStatModifier(mod);
    }

    public void PickUpItem(IPickupable item)
    {
        Inventory.AddItem(item.GetItemData());
        item.OnPickup();
    }

    [PunRPC]
    public void RPC_SetAnimation(string triggerName)
    {
        Animator.SetTrigger(triggerName);

        Debug.Log($"{PhotonView.ViewID} : animation - {triggerName}");
    }

    public void SetHighlight(bool isOn)
    {
        HighlightEffect.SetHighlighted(isOn);
    }
}