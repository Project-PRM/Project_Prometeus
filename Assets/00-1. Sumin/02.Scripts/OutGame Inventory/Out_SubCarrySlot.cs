using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Out_SubCarrySlot : Out_CarrySlot, IPointerClickHandler
{
    private Out_EquipmentSubPanelController _controller;
    public int SlotIndex { get; set; } // CarrySlot과 동기화용 인덱스

    public void SetItem(ItemData data, Out_EquipmentSubPanelController controller, int slotIndex = -1)
    {
        _item = data;
        _itemNameText.text = _item != null ? _item.Name : "Empty";
        _controller = controller;
        if (slotIndex >= 0) SlotIndex = slotIndex;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        _controller.SwapWithOrigin(this);
        Out_CarryPanel.Instance.HideSubPanel();
    }
}
