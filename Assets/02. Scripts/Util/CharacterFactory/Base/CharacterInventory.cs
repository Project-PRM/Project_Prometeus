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
        for(int i=0; i<4; i++)
        {
            if(ItemManager.Instance.TryGetItemData("TestArmor", out var armor))
            {
                AddItem(armor);
            }
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
        if (!HavingItems.Contains(item))
            return false;

        // 같은 타입의 장비가 이미 장착되어 있다면 먼저 해제
        var sameTypeItems = EquippedItems.Where(i => i.ItemType == item.ItemType).ToList();

        int maxAllowed = GetMaxAllowed(item.ItemType);
        if (sameTypeItems.Count >= maxAllowed)
        {
            // 초과 시 하나 교체 (혹은 다른 교체 정책 적용)
            var toReplace = sameTypeItems.First(); // 임의로 첫 번째 제거
            EquippedItems.Remove(toReplace);
            HavingItems.Add(toReplace);
        }

        HavingItems.Remove(item);
        EquippedItems.Add(item);
        _character.AddStatModifier(item.ToStatModifier());
        return true;
    }

    private int GetMaxAllowed(EItemType type)
    {
        switch (type)
        {
            case EItemType.Armor:
                return 3;
            default:
                return 1;
        }
    }

    public bool TryUnequipItem(ItemData item)
    {
        if (!EquippedItems.Contains(item)) return false;

        EquippedItems.Remove(item);
        HavingItems.Add(item);

        _character.RemoveStatModifier(item.ToStatModifier());
        return true;
    }
}
