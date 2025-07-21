using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 유저가 보유중인 아이템 데이터베이스
/// </summary>
public class UserInventoryItemManager : Singleton<UserInventoryItemManager>
{
    private UserInventoryItemRepository _repository;
    private List<UserInventoryItem> _items;
    public List<UserInventoryItem> Items => _items;

    protected async override void Awake()
    {
        _repository = new UserInventoryItemRepository();
        await GetInventoryAsync("InventoryTestUser");
    }

    public async Task GetInventoryAsync(string userId)
    {
        _items = await _repository.GetInventoryAsync(userId);

        foreach(var item in _items)
        {
            Debug.Log($"ItemId: {item.ItemId}, Count: {item.Count}");
        }
    }

    public async Task SetInventoryAsync(List<UserInventoryItem> itemsToSave)
    {
        await _repository.SetInventoryAsync(AccountManager.Instance.MyAccount.UserId, itemsToSave);
        await GetInventoryAsync(AccountManager.Instance.MyAccount.UserId); // 저장 후 다시 로드
    }
}
