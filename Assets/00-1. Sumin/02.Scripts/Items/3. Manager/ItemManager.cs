using System.Collections.Generic;
using System.Threading.Tasks;

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