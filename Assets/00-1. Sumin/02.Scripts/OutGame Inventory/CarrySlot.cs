using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CarrySlot : ItemSlotBase, IPointerClickHandler
{
    [SerializeField] protected TextMeshProUGUI _itemNameText;
    [SerializeField] private EItemType _allowedType;
    public EItemType AllowedType => _allowedType;

    public override void SetItem(ItemData newItem)
    {
        base.SetItem(newItem);
        if (CarryPanel.Instance != null)
            CarryPanel.Instance.SyncCarryAndSubCarrySlots();
    }

    public override bool CanAccept(ItemData item)
    {
        return item != null && item.ItemType == _allowedType;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && _item != null)
        {
            InventoryPanel.Instance.AddItemToInventory(_item);
            SetItem(null); // CarrySlot 비움
        }
    }

    protected override void UpdateVisual()
    {
        // 아이콘 및 희귀도 테두리
        if (_itemNameText != null)
            _itemNameText.text = _item?.Name ?? "Empty";
    }
}
