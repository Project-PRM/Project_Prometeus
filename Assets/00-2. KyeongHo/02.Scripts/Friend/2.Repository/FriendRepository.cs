using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine;

public class FriendRepository
{
    private const string FRIEND_REQUESTS = "FriendRequests";
    private const string COLLECTION_NAME = "UserAccount";
    private const string FRIENDS = "Friends";
    private CollectionReference _userCollection => FirebaseInitialize.DB.Collection(COLLECTION_NAME);

    public async Task<List<FriendRequestDto>> GetFriendRequestsAsync(string userUid)
    {
        await FirebaseInitialize.WaitForInitializationAsync();
        
        var doc = await _userCollection.Document(userUid).GetSnapshotAsync();
        return doc.TryGetValue(FRIEND_REQUESTS, out List<string> uids)
            ? uids.Select(uid => new FriendRequestDto(uid)).ToList()
            : new List<FriendRequestDto>();
    }

    public async Task AddFriendRequestAsync(string recipientUid, FriendRequestDto request)
    {
        await FirebaseInitialize.WaitForInitializationAsync();
        
        await _userCollection.Document(recipientUid)
            .UpdateAsync(FRIEND_REQUESTS, FieldValue.ArrayUnion(request.SenderUid));
    }

    public async Task RemoveFriendRequestAsync(string recipientUid, FriendRequestDto request)
    {
        await FirebaseInitialize.WaitForInitializationAsync();
        
        await _userCollection.Document(recipientUid)
            .UpdateAsync(FRIEND_REQUESTS, FieldValue.ArrayRemove(request.SenderUid));
    }

    public async Task<List<string>> GetFriendsAsync(string userUid)
    {
        await FirebaseInitialize.WaitForInitializationAsync();
        
        var doc = await _userCollection.Document(userUid).GetSnapshotAsync();
        return doc.TryGetValue(FRIENDS, out List<string> list) ? list : new List<string>();
    }

    public async Task AddFriendAsync(string userUid, string friendUid)
    {
        await FirebaseInitialize.WaitForInitializationAsync();
        
        WriteBatch batch = FirebaseInitialize.DB.StartBatch();
        var userDoc = _userCollection.Document(userUid);
        var friendDoc = _userCollection.Document(friendUid);

        batch.Update(userDoc, new Dictionary<string, object>
        {
            { FRIENDS, FieldValue.ArrayUnion(friendUid) }
        });

        batch.Update(friendDoc, new Dictionary<string, object>
        {
            { FRIENDS, FieldValue.ArrayUnion(userUid) }
        });

        await batch.CommitAsync();
    }

    public async Task RemoveFriendAsync(string userUid, string friendUid)
    {
        await FirebaseInitialize.WaitForInitializationAsync();
        
        WriteBatch batch = FirebaseInitialize.DB.StartBatch();
        var userDoc = _userCollection.Document(userUid);
        var friendDoc = _userCollection.Document(friendUid);

        batch.Update(userDoc, new Dictionary<string, object>
        {
            { FRIENDS, FieldValue.ArrayRemove(friendUid) }
        });

        batch.Update(friendDoc, new Dictionary<string, object>
        {
            { FRIENDS, FieldValue.ArrayRemove(userUid) }
        });

        await batch.CommitAsync();
    }
}