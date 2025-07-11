using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CharacterBehaviour : PlayerActivity
{
    [SerializeField] private ECharacterName _characterName;

    private CharacterBase _character;

    private bool _isInitialized = false;

    // 캐릭터 스킬 조합
    protected override async void Start()
    {
        base.Start();
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
        if (!_photonView.IsMine) return;

        if (callback.performed)
        {
            // 일반 공격을 함
            Debug.Log("Normal Attack performed");
            _character.UseSkill(ESkillType.BasicAttack);
            _photonView.RPC(nameof(RPC_NormalAttack), RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_NormalAttack()
    {
        Debug.Log("Normal Attack performed via RPC");
        _character.UseSkill(ESkillType.BasicAttack);
    }

    private void Update()
    {
        if(_isInitialized == false)
        {
            return;
        }
        //_character.Update();

        //if (Input.GetMouseButtonDown(0))
        //{
        //    _character.UseSkill(ESkillType.BasicAttack);
        //}
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _character.UseSkill(ESkillType.Skill);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _character.UseSkill(ESkillType.Ultimate);
        }
    }
}
