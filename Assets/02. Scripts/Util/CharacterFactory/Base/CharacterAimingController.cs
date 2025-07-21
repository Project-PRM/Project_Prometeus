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
        if (!IsAiming)
        {
            if (Input.GetMouseButtonDown(0)) // 좌클릭
            {
                Vector3 mousePos = GetMouseWorldPosition();

                // 1. 클릭한 위치에서 아이템 있는지 먼저 체크
                if (TryPickupItemAt(mousePos))
                {
                    return; // 아이템 주웠으면 끝
                }
            }
            return;
        }        /*if (!_owner.PhotonView.IsMine)
        {
            return;
        }*/

        UpdateAimingUI();

        if (Input.GetMouseButtonDown(0)) // 좌클릭
        {
            Vector3 targetPoint = GetMouseWorldPosition();
            var target = GetTargetUnderMouse(targetPoint);
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

    private bool TryPickupItemAt(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, 0.5f);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IPickupable>(out var pickupable))
            {
                /*var inventory = _owner.GetCharacterBase().Inventory;
                if (pickupable.Pickup(inventory))
                {
                    // 아이템 주웠으면 오브젝트 파괴하거나 비활성화
                    GameObject.Destroy(hit.gameObject);
                    return true;
                }*/
            }
        }
        return false;
    }
}
