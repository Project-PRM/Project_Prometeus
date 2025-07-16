using TMPro;
using UnityEngine;

public class CarrySlot : ItemSlotBase
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

    protected override void UpdateVisual()
    {
        // 아이콘 및 희귀도 테두리
        if (_itemNameText != null)
            _itemNameText.text = _item?.Name ?? "Empty";
    }
}
