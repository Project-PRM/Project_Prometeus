using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyInventoryEquipSlot : ItemSlotBase, IPointerClickHandler
{
    [SerializeField] private EItemType _allowedType;
    public EItemType AllowedType => _allowedType;

    private float _lastClickTime = 0f;
    private const float DoubleClickThreshold = 0.3f;

    public override bool CanAccept(ItemData item)
    {
        return item != null && item.ItemType == _allowedType;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && _item != null)
        {
            if (Time.time - _lastClickTime < DoubleClickThreshold)
            {
                MyInventoryPanel.Instance.TryUnequipItem(_item);
                _lastClickTime = 0f;
            }
            else
            {
                _lastClickTime = Time.time;
            }
        }
    }
}
