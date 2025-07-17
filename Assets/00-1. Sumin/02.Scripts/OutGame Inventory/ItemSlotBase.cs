using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ItemSlotBase : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _itemNameText;
    [SerializeField] protected Image _icon;

    protected ItemData _item;

    public bool IsEmpty => _item == null;

    private void Awake()
    {
        if(_icon == null)
        {
            _icon = GetComponentInChildren<Image>();
        }
        _itemNameText = GetComponentInChildren<TextMeshProUGUI>();
    }

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
        Refresh();
    }

    public virtual ItemData GetItem() => _item;

    public virtual void ClearItem()
    {
        _item = null;
        Refresh();
    }

    // 슬롯에 따른 UI 처리 방식은 하위에서 구현
    protected abstract void Refresh();
}
