using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyInventorySlot : ItemSlotBase, IPointerClickHandler
{
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && _item != null)
        {
            MyInventoryPanel.Instance.TryEquipItem(_item);
        }
    }
}