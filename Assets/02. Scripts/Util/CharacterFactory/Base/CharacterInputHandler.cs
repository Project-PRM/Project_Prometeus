using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CharacterInputHandler : MonoBehaviour
{
    private CharacterBehaviour _characterBehaviour;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _characterBehaviour = GetComponent<CharacterBehaviour>();
        _playerInput = GetComponent<PlayerInput>();

        if (_characterBehaviour == null || _playerInput == null)
        {
            Debug.LogError("Missing required components on CharacterInputHandler.");
            return;
        }

        if (!_characterBehaviour.PhotonView.IsMine)
        {
            _playerInput.enabled = false; // 🔒 입력 비활성화
            enabled = false;              // 이 스크립트도 비활성화
        }
    }


    private void OnEnable()
    {
        if (!_characterBehaviour.PhotonView.IsMine)
        {
            return;
        }
        _playerInput.actions["Passive"].performed += OnPassiveUse;
        _playerInput.actions["Skill"].performed += OnSkillUse;
        _playerInput.actions["Ultimate"].performed += OnUltimateUse;
        _playerInput.actions["Attack"].performed += OnAttack;
    }

    private void OnDisable()
    {
        if (!_characterBehaviour.PhotonView.IsMine)
        {
            return;
        }
        _playerInput.actions["Passive"].performed -= OnPassiveUse;
        _playerInput.actions["Skill"].performed -= OnSkillUse;
        _playerInput.actions["Ultimate"].performed -= OnUltimateUse;
        _playerInput.actions["Attack"].performed -= OnAttack;
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        _characterBehaviour.OnAttack(context);
    }

    private void OnSkillUse(InputAction.CallbackContext context)
    {
        _characterBehaviour.OnSkillUse(context);
    }

    private void OnUltimateUse(InputAction.CallbackContext context)
    {
        _characterBehaviour.OnUltimateUse(context);
    }

    private void OnPassiveUse(InputAction.CallbackContext context)
    {
        _characterBehaviour.OnPassiveUse(context);
    }
}
