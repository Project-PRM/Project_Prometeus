using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// 모든 아이템들에 대한 데이터베이스
/// </summary>
public class ItemManager : Singleton<ItemManager>
{
    private ItemRepository _itemRepository;

    public Dictionary<string, ItemData> Items => _itemRepository.Items;

    protected async override void Awake()
    {
        _itemRepository = new ItemRepository();
        await _itemRepository.InitializeAsync();
    }

    public async Task<ItemData> GetItemDataAsync(string itemName)
    {
        if (_itemRepository.Initialized == false)
        {
            await _itemRepository.InitializeAsync();
        }
        return await _itemRepository.GetItemDataAsync(itemName);
    }

    public bool TryGetItemData(string itemName, out ItemData itemData)
    {
        return _itemRepository.Items.TryGetValue(itemName, out itemData);
    }

    public List<ItemData> GetAllItems()
    {
        return new List<ItemData>(_itemRepository.Items.Values);
    }
}