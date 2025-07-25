using UnityEngine;
using System;

public class CharacterAimingController
{
    private readonly CharacterBehaviour _character;
    private ESkillType? _selectedSkill = null;
    private ESkillInputState _state = ESkillInputState.None;

    private Vector3 _aimDirection;
    public Vector3 AimDirection => _aimDirection;

    public bool IsAiming => _state == ESkillInputState.Aiming;

    public CharacterAimingController(CharacterBehaviour owner)
    {
        _character = owner;
    }

    public void EnterAimingMode(ESkillType skillType)
    {
        _selectedSkill = skillType;
        _state = ESkillInputState.Aiming;
        // TODO: 에임 UI 보이기
    }

    public void Update()
    {
        /*if (!_owner.PhotonView.IsMine)
        {
            return;
        }*/

        if (!IsAiming)
        {
            if (Input.GetMouseButtonDown(0)) // 좌클릭
            {
                IPickupable pickupable = MouseSelector.Instance.GetPickupableUnderMouse();
                if (pickupable != null)
                {
                    _character.PickUpItem(pickupable);
                    return;
                }
            }
            return;
        }

        UpdateAimingUI();

        if (Input.GetMouseButtonDown(0)) // 좌클릭
        {
            CharacterBase target = MouseSelector.Instance.GetCharacterUnderMouse();
            Vector3 point = MouseSelector.Instance.GetMouseWorldPosition();

            _character.GetCharacterBase().UseSkill(_selectedSkill.Value, target, point);
            Reset();
        }

        if (Input.GetMouseButtonDown(1)) // 우클릭
        {
            Reset();
        }
    }

    private void UpdateAimingUI()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        _aimDirection = (mouseWorld - _character.transform.position).normalized;

        // TODO: 에임 UI 회전, 위치 갱신
    }

    public void Reset()
    {
        _selectedSkill = null;
        _state = ESkillInputState.None;
        // TODO: 에임 UI 끄기
    }
}