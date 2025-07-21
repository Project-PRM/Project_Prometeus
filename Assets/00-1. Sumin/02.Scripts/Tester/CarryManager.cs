using System.Collections.Generic;
using UnityEngine;

public class CarryManager : Singleton<CarryManager>
{
    // 인-아웃게임을 넘나드는 아이템들
    public List<ItemData> CarryItems { get; private set; } = new List<ItemData>();

    public void SetCarryingList(List<ItemData> items)
    {
        CarryItems.Clear();
        CarryItems.AddRange(items);
        Debug.Log($"Carrying items updated. Total items: {CarryItems.Count}");
    }

    public List<ItemData> GetCarryableItemsExcludingEquipped(CharacterInventory inventory)
    {
        var carryableItems = new List<ItemData>(inventory.HavingItems);

        foreach (var equipped in inventory.EquippedItems)
        {
            carryableItems.Remove(equipped); // 같은 참조일 경우 동작
        }

        return carryableItems;
    }
}
