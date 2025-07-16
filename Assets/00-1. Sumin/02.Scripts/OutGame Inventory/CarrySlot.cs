using UnityEngine;

public class CarrySlot : ItemSlotBase
{
    [SerializeField] private EItemType _allowedType;
    public EItemType AllowedType => _allowedType;

    public override bool CanAccept(ItemData item)
    {
        return item != null && item.ItemType == _allowedType;
    }

    protected override void UpdateVisual()
    {
        // 아이콘 및 희귀도 테두리
    }
}
