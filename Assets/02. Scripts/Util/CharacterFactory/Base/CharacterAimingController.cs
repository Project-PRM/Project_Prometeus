using UnityEngine;
using System;

public class CharacterAimingController
{
    private readonly CharacterBehaviour _owner;
    private ESkillType? _selectedSkill = null;
    private ESkillInputState _state = ESkillInputState.None;

    private Vector3 _aimDirection;
    public Vector3 AimDirection => _aimDirection;

    public bool IsAiming => _state == ESkillInputState.Aiming;

    public CharacterAimingController(CharacterBehaviour owner)
    {
        _owner = owner;
    }

    public void EnterAimingMode(ESkillType skillType)
    {
        _selectedSkill = skillType;
        _state = ESkillInputState.Aiming;
        // TODO: 에임 UI 보이기
    }

    public void Update()
    {
        if (!IsAiming) return;

        UpdateAimingUI();

        if (Input.GetMouseButtonDown(0)) // 좌클릭
        {
            Vector3 targetPoint = GetMouseWorldPosition();
            var target = _owner.GetTargetUnderMouse(targetPoint);
            _owner.GetCharacterBase().UseSkill(_selectedSkill.Value, target, targetPoint);
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

        _aimDirection = (mouseWorld - _owner.transform.position).normalized;

        // TODO: 에임 UI 회전, 위치 갱신
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        return groundPlane.Raycast(ray, out float enter)
            ? ray.GetPoint(enter)
            : ray.GetPoint(10f);
    }

    public void Reset()
    {
        _selectedSkill = null;
        _state = ESkillInputState.None;
        // TODO: 에임 UI 끄기
    }
}
