using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine;

public class FriendRepository
{
    private CollectionReference _nicknameCollection => FirebaseInitialize.DB.Collection("Nicknames");

    public async Task<List<FriendRequestDto>> GetFriendRequestsAsync(string userUid)
    {
        var doc = await _nicknameCollection.Document(userUid).GetSnapshotAsync();
        return doc.TryGetValue("FriendRequests", out List<string> uids)
            ? uids.Select(uid => new FriendRequestDto(uid)).ToList()
            : new List<FriendRequestDto>();
    }

    public async Task AddFriendRequestAsync(string recipientUid, FriendRequestDto request)
    {
        await _nicknameCollection.Document(recipientUid)
            .UpdateAsync("FriendRequests", FieldValue.ArrayUnion(request.SenderUid));
    }

    public async Task RemoveFriendRequestAsync(string recipientUid, FriendRequestDto request)
    {
        await _nicknameCollection.Document(recipientUid)
            .UpdateAsync("FriendRequests", FieldValue.ArrayRemove(request.SenderUid));
    }

    public async Task<List<string>> GetFriendsAsync(string userUid)
    {
        var doc = await _nicknameCollection.Document(userUid).GetSnapshotAsync();
        return doc.TryGetValue("Friends", out List<string> list) ? list : new List<string>();
    }

    public async Task AddFriendAsync(string userUid, string friendUid)
    {
        WriteBatch batch = FirebaseInitialize.DB.StartBatch();
        var userDoc = _nicknameCollection.Document(userUid);
        var friendDoc = _nicknameCollection.Document(friendUid);

        batch.Update(userDoc, new Dictionary<string, object>
        {
            { "Friends", FieldValue.ArrayUnion(friendUid) }
        });

        batch.Update(friendDoc, new Dictionary<string, object>
        {
            { "Friends", FieldValue.ArrayUnion(userUid) }
        });

        await batch.CommitAsync();
    }

    public async Task RemoveFriendAsync(string userUid, string friendUid)
    {
        WriteBatch batch = FirebaseInitialize.DB.StartBatch();
        var userDoc = _nicknameCollection.Document(userUid);
        var friendDoc = _nicknameCollection.Document(friendUid);

        batch.Update(userDoc, new Dictionary<string, object>
        {
            { "Friends", FieldValue.ArrayRemove(friendUid) }
        });

        batch.Update(friendDoc, new Dictionary<string, object>
        {
            { "Friends", FieldValue.ArrayRemove(userUid) }
        });

        await batch.CommitAsync();
    }
}
