using System.Collections.Generic;
using UnityEngine;

public class Out_UserInventoryPanel : Singleton<Out_UserInventoryPanel>
{
    [SerializeField] private Transform _slotParent; // 슬롯들을 넣을 부모 오브젝트
    [SerializeField] private Out_UserInventorySlot _slotPrefab; // 슬롯 프리팹

    private List<Out_UserInventorySlot> _slots = new();

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        var flatItemList = new List<ItemData>();

        // 1. 유저 인벤토리에서 아이템을 개수만큼 풀어냄
        foreach (var invItem in UserInventoryItemManager.Instance.Items)
        {
            if (ItemManager.Instance.TryGetItemData(invItem.ItemId, out var itemData))
            {
                for (int i = 0; i < invItem.Count; i++)
                {
                    flatItemList.Add(itemData);
                }
            }
        }

        // 2. CarryManager에서 이미 선택된 아이템은 제거
        var carryItems = new List<ItemData>(CarryManager.Instance.CarryItems); // 수정 방지용 복사

        foreach (var carryItem in carryItems)
        {
            var match = flatItemList.Find(i => i.Name == carryItem.Name);
            if (match != null)
            {
                flatItemList.Remove(match); // 같은 Id 하나 제거
            }
        }

        // 3. 슬롯 개수 동기화
        while (_slots.Count < flatItemList.Count)
        {
            var newSlot = Instantiate(_slotPrefab, _slotParent);
            _slots.Add(newSlot);
        }

        while (_slots.Count > flatItemList.Count)
        {
            var lastSlot = _slots[_slots.Count - 1];
            _slots.RemoveAt(_slots.Count - 1);
            DestroyImmediate(lastSlot.gameObject);
        }

        // 4. 슬롯에 아이템 할당
        for (int i = 0; i < flatItemList.Count; i++)
        {
            _slots[i].SetItem(flatItemList[i]);
        }
    }

    public void RemoveSlot(Out_UserInventorySlot slot)
    {
        _slots.Remove(slot);
    }

    public void AddItemToInventory(ItemData item)
    {
        // 빈 슬롯 찾기
        foreach (var slot in _slots)
        {
            if (slot.IsEmpty)
            {
                slot.SetItem(item);
                return;
            }
        }
        // 빈 슬롯이 없으면 새로 생성
        var newSlot = Instantiate(_slotPrefab, _slotParent);
        _slots.Add(newSlot);
        newSlot.SetItem(item);
    }
}
