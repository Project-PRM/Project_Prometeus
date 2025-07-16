using Firebase.Firestore;

[FirestoreData]
public class UserInventoryItem
{
    [FirestoreProperty] public string ItemId { get; set; }  // ex: "Sword001"
    [FirestoreProperty] public int Count { get; set; }      // 소지 개수

    public UserInventoryItem() { }

    public UserInventoryItem(string itemId, int count)
    {
        ItemId = itemId;
        Count = count;
    }
}
