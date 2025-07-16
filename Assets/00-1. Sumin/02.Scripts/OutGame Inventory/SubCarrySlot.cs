using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SubCarrySlot : CarrySlot, IPointerClickHandler
{
    private EquipmentPanelRootController _controller;
    public int SlotIndex { get; set; } // CarrySlot과 동기화용 인덱스

    public void SetItem(ItemData data, EquipmentPanelRootController controller, int slotIndex = -1)
    {
        _item = data;
        _itemNameText.text = _item != null ? _item.Name : "Empty";
        _controller = controller;
        if (slotIndex >= 0) SlotIndex = slotIndex;

        // 안전하게 null 확인 후 디버그
        //Debug.Log($"{gameObject.name} SetItem called with item: {(_item != null ? _item.Name : "null")}");
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        _controller.SwapWithOrigin(this);
    }
}
