using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ItemRepository
{
    private FirebaseFirestore _db => FirebaseInitialize.DB;

    public Dictionary<string, ItemData> Items { get; private set; } = new();

    public bool Initialized { get; private set; } = false;

    public async Task InitializeAsync()
    {
        if (Initialized) return;
        Initialized = true;

        Items.Clear();
        await GetAllItemDataAsync();
    }

    public async Task<ItemData> GetItemDataAsync(string itemName)
    {
        if (Items.TryGetValue(itemName, out var cached))
            return cached;

        await FirebaseInitialize.WaitForInitializationAsync();

        var docRef = _db.Collection("ItemDatas").Document(itemName);
        var snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            var item = snapshot.ConvertTo<ItemData>();
            Items[item.Name] = item;
            return item;
        }
        else
        {
            Debug.LogError("아이템 데이터가 존재하지 않습니다.");
            return null;
        }
    }

    private async Task GetAllItemDataAsync()
    {
        await FirebaseInitialize.WaitForInitializationAsync();

        var collectionRef = _db.Collection("ItemDatas");
        var querySnapshot = await collectionRef.GetSnapshotAsync();

        foreach (var docSnap in querySnapshot.Documents)
        {
            if (docSnap.Exists)
            {
                var itemData = docSnap.ConvertTo<ItemData>();
                Items[itemData.Name] = itemData;
            }
        }
    }
}
