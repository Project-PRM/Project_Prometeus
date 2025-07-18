using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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

    protected override void Refresh()
    {
        // 아이콘 및 희귀도 테두리
        if (_itemNameText != null)
            _itemNameText.text = _item?.Name ?? "Empty";

        if (_item != null)
        {
            _icon.sprite = _item.IconSprite;
        }
        else
        {
            // 빈 슬롯 처리
            _icon.sprite = null;
            _icon.color = Color.white;
        }
    }
}
