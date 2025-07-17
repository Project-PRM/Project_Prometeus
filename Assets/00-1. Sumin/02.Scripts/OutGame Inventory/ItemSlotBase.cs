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
        if(_icon != null)
        {
            _icon.color = Color.white; // 빈 슬롯의 아이콘 색상 초기화
            _icon.sprite = null;
        }    
        if(_itemNameText != null)
            _itemNameText.text = "";

        Refresh();
    }


    // 슬롯에 따른 UI 처리 방식은 하위에서 구현
    protected abstract void Refresh();
}
