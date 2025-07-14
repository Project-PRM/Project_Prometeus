using System.Collections.Generic;

public class EquipmentSet
{
    private List<EquipmentItem> _equippedItems = new List<EquipmentItem>();

    public void Equip(EquipmentItem item)
    {
        // 슬롯 교체 등 로직 처리
        _equippedItems.Add(item);
    }

    public List<EquipmentItem> GetAllEquipped()
    {
        return _equippedItems;
    }
}
