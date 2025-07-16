using TMPro;
using UnityEngine;

public abstract class ItemSlotBase : MonoBehaviour
{
    protected ItemData _item;
    [SerializeField] protected TextMeshProUGUI _itemNameText;

    public bool IsEmpty => _item == null;

    private void Awake()
    {
        if(_itemNameText == null)
        {
            //_itemNameText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public virtual bool CanAccept(ItemData item)
    {
        return item != null;
    }

    public virtual void SetItem(ItemData newItem)
    {
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
