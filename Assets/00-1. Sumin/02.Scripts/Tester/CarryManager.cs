using System.Collections.Generic;
using UnityEngine;

public class CarryManager : Singleton<CarryManager>
{
    public List<ItemData> CarryItems { get; private set; } = new List<ItemData>();

    public void SetCarryingList(List<ItemData> items)
    {
        CarryItems.Clear();
        CarryItems.AddRange(items);
        Debug.Log($"Carrying items updated. Total items: {CarryItems.Count}");
    }
}
