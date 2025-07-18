using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 들고가는 장비 슬롯
/// </summary>
public class Out_CarrySlot : ItemSlotBase, IPointerClickHandler
{
    [SerializeField] private EItemType _allowedType;
    public EItemType AllowedType => _allowedType;

    public override void SetItem(ItemData newItem)
    {
        base.SetItem(newItem);
        if (Out_CarryPanel.Instance != null)
            Out_CarryPanel.Instance.SyncCarryAndSubCarrySlots();
    }

    public override bool CanAccept(ItemData item)
    {
        return item != null && item.ItemType == _allowedType;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && _item != null)
        {
            Out_UserInventoryPanel.Instance.AddItemToInventory(_item);
            SetItem(null); // CarrySlot 비움
        }
    }
}
