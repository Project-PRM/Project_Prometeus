using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : ItemSlotBase, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _itemNameText;
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
            CarryPanel.Instance.CanvasRectTransform, // 캔버스의 RectTransform
            eventData.position, // 마우스 클릭 위치
            eventData.pressEventCamera, // 클릭 이벤트 카메라 (Screen Space - Camera일 경우 필요)
            out Vector2 localClickPosition
        );

        CarryPanel.Instance.ShowNear(localClickPosition, _item);
        // 자신을 OriginSlot으로 지정
        EquipmentPanelRootController controller = FindAnyObjectByType<EquipmentPanelRootController>();
        if (controller != null)
            controller.SetOriginSlot(this);
    }

    protected override void UpdateVisual()
    {
        if (_itemNameText != null)
            _itemNameText.text = _item?.Name ?? "Empty";

        if (_item == null && !_isSubSlot)
        {
            // InventoryPanel에서 자신을 리스트에서 제거
            if (InventoryPanel.Instance != null)
                InventoryPanel.Instance.RemoveSlot(this);
            Destroy(gameObject);
        }
    }
}
