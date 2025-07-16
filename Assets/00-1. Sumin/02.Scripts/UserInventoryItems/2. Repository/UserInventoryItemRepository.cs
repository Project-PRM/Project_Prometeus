using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UserInventoryItemRepository
{
    private const string COLLECTIONNAME = "UserInventoryItems";

    private FirebaseFirestore _db => FirebaseInitialize.DB;

    public async Task<List<UserInventoryItem>> GetInventoryAsync(string userId)
    {
        await FirebaseInitialize.WaitForInitializationAsync();

        var docRef = _db.Collection(COLLECTIONNAME).Document(userId);
        var snapshot = await docRef.GetSnapshotAsync();

        List<UserInventoryItem> result = new();

        if (!snapshot.Exists)
            return result;

        // 필드 이름(key)과 값(value)을 순회하며 UserInventoryItem 리스트로 변환
        foreach (var field in snapshot.ToDictionary())
        {
            string itemId = field.Key;
            int count = 0;

            // 필드 값이 int 혹은 long으로 올 수 있으니 변환 처리
            if (field.Value is long l) count = (int)l;
            else if (field.Value is int i) count = i;
            else
            {
                // 예상하지 못한 타입이면 무시하거나 로그 남기기
                UnityEngine.Debug.LogWarning($"Invalid count type for item '{itemId}': {field.Value?.GetType()}");
                continue;
            }

            result.Add(new UserInventoryItem(itemId, count));
        }

        return result;
    }

    public async Task SetInventoryAsync(string userId, List<UserInventoryItem> inventory)
    {
        await FirebaseInitialize.WaitForInitializationAsync();

        var docRef = _db.Collection(COLLECTIONNAME).Document(userId);

        var data = new Dictionary<string, object>();

        foreach (var item in inventory)
        {
            if (item.Count > 0)
            {
                data[item.ItemId] = item.Count;
            }
        }

        await docRef.SetAsync(data);
    }
}
