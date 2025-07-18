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

    public void ClearItem()
    {
        _item = null;

        Refresh();
    }

    protected virtual void Refresh()
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
