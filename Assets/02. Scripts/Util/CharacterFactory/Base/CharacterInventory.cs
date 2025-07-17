using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterInventory : MonoBehaviour
{
    public List<ItemData> HavingItems = new();
    public List<ItemData> EquippedItems = new();

    private CharacterBehaviour _character;

    private void Awake()
    {
        _character = GetComponent<CharacterBehaviour>();
    }

    private void Start()
    {
        Invoke(nameof(InitializeInventory), 1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            var panel = MyInventoryPanel.Instance;
            if (panel.gameObject.activeSelf)
            {
                panel.gameObject.SetActive(false);
            }
            else
            {
                panel.OpenPanel(this);
            }
        }
    }

    private void InitializeInventory()
    {
        HavingItems.Clear();
        EquippedItems.Clear();

        if (ItemManager.Instance.TryGetItemData("TestItem", out var item))
        {
            AddItem(item);
        }
    }

    public void AddItem(ItemData item)
    {
        HavingItems.Add(item);
    }

    public void RemoveItem(ItemData item)
    {
        HavingItems.Remove(item);
    }

    public bool TryEquipItem(ItemData item)
    {
        if (!HavingItems.Contains(item)) return false;

        // 장착 위치에 이미 있는 동일 타입 아이템 제거
        switch (item.ItemType)
        {
            case EItemType.Weapon:
            case EItemType.Bag:
            case EItemType.Light:
                UnequipSameType(item.ItemType); // 덮어쓰기 위해 기존 장비 제거
                break;
            case EItemType.Armor:
                if (EquippedItems.Count(i => i.ItemType == EItemType.Armor) >= 3) // 예시
                    return false;
                break;
        }

        HavingItems.Remove(item);
        EquippedItems.Add(item);

        _character.AddStatModifier(item.ToStatModifier());
        return true;
    }

    public bool TryUnequipItem(ItemData item)
    {
        if (!EquippedItems.Contains(item)) return false;

        EquippedItems.Remove(item);
        HavingItems.Add(item);

        _character.RemoveStatModifier(item.ToStatModifier());
        return true;
    }

    private void UnequipSameType(EItemType type)
    {
        var same = EquippedItems.FirstOrDefault(i => i.ItemType == type);
        if (same != null)
        {
            EquippedItems.Remove(same);
            HavingItems.Add(same);
        }
    }

}
