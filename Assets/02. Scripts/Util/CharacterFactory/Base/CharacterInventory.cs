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
        if (!_character.PhotonView.IsMine)
        {
            return;
        }
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

        // 캐리 아이템 불러오기
        var initialItems = CarryManager.Instance.CarryItems;

        foreach (var item in initialItems)
        {
            AddItem(item);
            // 장비 아이템이면 자동 장착 시도
            if (item.ItemType == EItemType.Weapon || item.ItemType == EItemType.Armor)
            {
                // TryEquipItem 내부에서 HavingItems를 다시 넣기 때문에 예외 처리 필요 없음
                TryEquipItem(item);
            }
        }

        // 예시용 추가 아이템 (테스트 목적)
        if (ItemManager.Instance.TryGetItemData("TestItem", out var testItem))
        {
            AddItem(testItem);
        }

        for (int i = 0; i < 4; i++)
        {
            if (ItemManager.Instance.TryGetItemData("TestArmor", out var armor))
            {
                AddItem(armor);
            }
        }

        Debug.Log($"CharacterInventory initialized with {HavingItems.Count} items in bag, {EquippedItems.Count} equipped.");
    }

    public void AddItem(ItemData item)
    {
        HavingItems.Add(item);
        SortHavingItems();
    }

    public void DropItem(ItemData item)
    {
        HavingItems.Remove(item);
        SortHavingItems();
    }

    public void DropAndInstantiateItem(ItemData item)
    {
        if (!HavingItems.Contains(item)) return;
        HavingItems.Remove(item);
        SortHavingItems();
        GameObject temp = Resources.Load<GameObject>($"Items/{item.Name}");
        GameObject itemObject = Instantiate(temp, transform.position, Quaternion.identity);
        //GameObject itemObject = PhotonNetwork.Instantiate($"Items/{item.Name}", transform.position, Quaternion.identity);
        itemObject.GetComponent<ItemBase>().SpreadFrom(transform.position);
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
        SortHavingItems();
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
        SortHavingItems();
        _character.RemoveStatModifier(item.ToStatModifier());
        return true;
    }

    private void SortHavingItems()
    {
        HavingItems = HavingItems
            .OrderBy(i => i.ItemType)
            .ThenBy(i => i.Name)
            .ToList();
    }
}
