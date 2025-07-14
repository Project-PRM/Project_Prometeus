using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CharacterBehaviour : /*PlayerActivity,*/MonoBehaviour, IStatusAffectable
{
    [SerializeField] private ECharacterName _characterName;

    private CharacterBase _character;

    private bool _isInitialized = false;

    private List<IStatusEffect> _activeEffects = new();

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

    private void Update()
    {
        if(_isInitialized == false)
        {
            return;
        }
        _character.Update();

        if (Input.GetMouseButtonDown(0))
        {
            _character.UseSkill(ESkillType.BasicAttack);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _character.UseSkill(ESkillType.Skill);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _character.UseSkill(ESkillType.Ultimate);
        }
    }

    public void ApplyEffect(IStatusEffect effect)
    {
        effect.Apply(_character);
        _activeEffects.Add(effect);
        StartCoroutine(RemoveEffectAfterTime(effect));
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
