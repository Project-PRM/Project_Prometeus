using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MyInventoryPanel : Singleton<MyInventoryPanel>
{
    private const int DEFAULT_INVENTORY_SIZE = 10;

    private CharacterInventory _currentInventory;

    [Header("장비 슬롯")]
    [SerializeField] private MyInventoryEquipSlot _weaponSlot;
    [SerializeField] private List<MyInventoryEquipSlot> _armorSlots;
    [SerializeField] private MyInventoryEquipSlot _bagSlot;
    [SerializeField] private MyInventoryEquipSlot _lightSlot;
    [SerializeField] private RectTransform _equipSlotsContainer;
    [SerializeField] private RectTransform _inventorySlotsContainer;

    [Header("소지품 슬롯")]
    [SerializeField] private List<MyInventorySlot> _inventorySlots;

    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }

    public void OpenPanel(CharacterInventory inventory)
    {
        _currentInventory = inventory;

        gameObject.SetActive(true);

        SetEquipSlots(inventory.EquippedItems);
        SetInventorySlots(inventory.HavingItems);

        UpdateBagSlotCount(); // 여기에만 한 번
    }

    /// <summary>
    /// 인벤토리에서 불러와서 UI 할당
    /// </summary>
    public void SetEquipSlots(List<ItemData> equipItems)
    {
        _weaponSlot.ClearItem();
        _bagSlot.ClearItem();
        _lightSlot.ClearItem();
        foreach (var armorSlot in _armorSlots)
            armorSlot.ClearItem();

        foreach (var item in equipItems)
        {
            switch (item.ItemType)
            {
                case EItemType.Weapon: _weaponSlot.SetItem(item); break;
                case EItemType.Bag: _bagSlot.SetItem(item); break;
                case EItemType.Light: _lightSlot.SetItem(item); break;
                case EItemType.Armor:
                    var emptyArmorSlot = _armorSlots.FirstOrDefault(s => s.IsEmpty);
                    if (emptyArmorSlot != null) emptyArmorSlot.SetItem(item);
                    break;
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(_equipSlotsContainer);
    }

    /// <summary>
    /// 인벤토리에서 불러와서 UI 할당
    /// </summary>
    public void SetInventorySlots(List<ItemData> inventoryItems)
    {
        for (int i = 0; i < _inventorySlots.Count; ++i)
        {
            if (i < inventoryItems.Count)
                _inventorySlots[i].SetItem(inventoryItems[i]);
            else
                _inventorySlots[i].ClearItem();
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(_inventorySlotsContainer);
    }

    /// <summary>
    /// 장착 시도
    /// </summary>
    public bool TryEquipItem(ItemData item)
    {
        if (_currentInventory.TryEquipItem(item))
        {
            OpenPanel(_currentInventory);
            UpdateBagSlotCount();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 해제 시도
    /// </summary>
    public bool TryUnequipItem(ItemData item)
    {
        if (_currentInventory.TryUnequipItem(item))
        {
            OpenPanel(_currentInventory);
            UpdateBagSlotCount();
            return true;
        }
        return false;
    }


    private void UpdateBagSlotCount()
    {
        var bagItem = _currentInventory.EquippedItems.FirstOrDefault(i => i.ItemType == EItemType.Bag);
        if (bagItem != null)
            OnBagEquipped(bagItem);
        else
            UpdateInventorySize(DEFAULT_INVENTORY_SIZE);
    }

    public void UpdateInventorySize(int maxSlotCount)
    {
        for (int i = 0; i < _inventorySlots.Count; ++i)
        {
            _inventorySlots[i].gameObject.SetActive(i < maxSlotCount);
        }
    }

    public void OnBagEquipped(ItemData bagItem)
    {
        int finalSlot = DEFAULT_INVENTORY_SIZE;
        if (bagItem.ItemType == EItemType.Bag && bagItem.InventorySlotCount > 0)
        {
            finalSlot = bagItem.InventorySlotCount;
        }

        UpdateInventorySize(finalSlot);
    }
}
