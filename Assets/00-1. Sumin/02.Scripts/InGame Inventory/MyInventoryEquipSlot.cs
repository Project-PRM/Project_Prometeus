using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyInventoryEquipSlot : ItemSlotBase, IPointerClickHandler
{
    [SerializeField] private EItemType _allowedType;
    public EItemType AllowedType => _allowedType;

    public override bool CanAccept(ItemData item)
    {
        return item != null && item.ItemType == _allowedType;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && _item != null)
        {
            MyInventoryPanel.Instance.TryMoveToInventory(_item);
        }
    }
}