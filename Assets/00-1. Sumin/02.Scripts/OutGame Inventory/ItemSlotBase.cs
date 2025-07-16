using TMPro;
using UnityEngine;

public abstract class ItemSlotBase : MonoBehaviour
{
    protected ItemData _item;

    public bool IsEmpty => _item == null;

    public virtual bool CanAccept(ItemData item)
    {
        return item != null;
    }

    public virtual void SetItem(ItemData newItem)
    {
        if (newItem == null)
        {
            ClearItem();
            return;
        }
        _item = newItem;
        UpdateVisual();
    }

    public virtual ItemData GetItem() => _item;

    public virtual void ClearItem()
    {
        _item = null;
        UpdateVisual();
    }

    // 슬롯에 따른 UI 처리 방식은 하위에서 구현
    protected abstract void UpdateVisual();
}
