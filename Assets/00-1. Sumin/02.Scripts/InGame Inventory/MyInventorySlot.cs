using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyInventorySlot : ItemSlotBase, IPointerClickHandler
{
    private float _lastClickTime = 0f;
    private const float DoubleClickThreshold = 0.3f;

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            float timeSinceLastClick = Time.time - _lastClickTime;

            if (timeSinceLastClick <= DoubleClickThreshold)
            {
                switch(_item.ItemType)
                {
                    case EItemType.Weapon:
                    case EItemType.Armor:
                        MyInventoryPanel.Instance.TryEquipItem(_item);
                        break;
                    case EItemType.Consumable:
                        //MyInventoryPanel.Instance.UseConsumable(_item);
                        break;
                    default:
                        Debug.LogWarning("Unknown item type for double click action.");
                        break;
                }
                _lastClickTime = 0f;
            }
            else
            {
                _lastClickTime = Time.time;
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            MyInventoryPanel.Instance.DropAndInstantiateItem(_item); // 아이템을 인벤토리에서 제거
        }
    }
}
