using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MyInventoryPanel : Singleton<MyInventoryPanel>
{
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

        // 가방이 있는 경우 슬롯 수 조절
        var bagItem = inventory.EquippedItems.FirstOrDefault(i => i.ItemType == EItemType.Bag);
        if (bagItem != null)
            OnBagEquipped(bagItem);
        else
            UpdateInventorySize(10); // 기본 슬롯 수 예시
    }

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

    public void SetInventorySlots(List<ItemData> inventoryItems)
    {
        for (int i = 0; i < _inventorySlots.Count; ++i)
        {
            if (i < inventoryItems.Count)
                _inventorySlots[i].SetItem(inventoryItems[i]);
            else
                _inventorySlots[i].ClearItem(); // 명시적으로 null 처리
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(_inventorySlotsContainer);
    }

    public bool TryEquipItem(ItemData item)
    {
        if (_currentInventory.TryEquipItem(item))
        {
            OpenPanel(_currentInventory); // 인벤토리 갱신

            if (item.ItemType == EItemType.Bag)
                OnBagEquipped(item);
            return true;
        }
        return false;
    }

    public bool TryMoveToInventory(ItemData item)
    {
        if (_currentInventory.TryUnequipItem(item))
        {
            OpenPanel(_currentInventory); // 인벤토리 갱신
            UpdateBagSlotCount(); // 가방 해제 시
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
            UpdateInventorySize(10);
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
        //int newSlotCount = bagItem.BagSlotCount;
        //UpdateInventorySize(newSlotCount);
    }
}
