using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Out_UserInventorySlot : ItemSlotBase, IPointerClickHandler
{
    [SerializeField] private bool _isSubSlot = false;

    public override void SetItem(ItemData newItem)
    {
        base.SetItem(newItem);
        // 아이콘 등 설정
        //Debug.Log($"{gameObject.name} SetItem called with item: {_item?.Name}");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 캔버스 기준 클릭 위치 전달
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Out_CarryPanel.Instance.CanvasRectTransform, // 캔버스의 RectTransform
            eventData.position, // 마우스 클릭 위치
            eventData.pressEventCamera, // 클릭 이벤트 카메라 (Screen Space - Camera일 경우 필요)
            out Vector2 localClickPosition
        );

        Out_CarryPanel.Instance.ShowNear(localClickPosition, _item);
        // 자신을 OriginSlot으로 지정
        Out_EquipmentSubPanelController controller = FindAnyObjectByType<Out_EquipmentSubPanelController>();
        if (controller != null)
            controller.SetOriginSlot(this);
    }

    protected override void Refresh()
    {
        // 아이콘 및 희귀도 테두리
        base.Refresh();

        if (_item == null && !_isSubSlot)
        {
            // InventoryPanel에서 자신을 리스트에서 제거
            Out_UserInventoryPanel.Instance.RemoveSlot(this);
            Destroy(gameObject);
        }
    }
}
