using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyInventorySlot : ItemSlotBase, IPointerClickHandler
{
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(gameObject.name + " clicked with button: " + eventData.button);
        if (eventData.button == PointerEventData.InputButton.Right && _item != null)
        {
            MyInventoryPanel.Instance.TryEquipItem(_item);
        }
    }
}