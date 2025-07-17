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
            if (MyInventoryPanel.Instance.TryEquipItem(_item))
                ClearItem();
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