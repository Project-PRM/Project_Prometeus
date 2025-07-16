
using UnityEngine;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;

// Firebase Firestore를 사용하여 친구 관계 및 요청을 관리하는 클래스입니다.
public class FirestoreFriendSystem : Singleton<FirestoreFriendSystem>
{
    // 다른 사용자에게 친구 요청을 보냅니다.
    // senderUid: 요청을 보내는 사용자 UID
    // recipientUid: 요청을 받을 사용자의 UID
    public async Task SendFriendRequestAsync(string senderUid, string recipientUid)
    {
        DocumentReference recipientDoc = FirebaseInitialize.DB.Collection("Nicknames").Document(recipientUid);

        // 이미 요청 보냈는지 확인
        DocumentSnapshot snapshot = await recipientDoc.GetSnapshotAsync();
        if (snapshot.TryGetValue("FriendRequests", out List<Dictionary<string, object>> requests))
        {
            bool alreadyRequested = requests.Any(r =>
                r.TryGetValue("senderUid", out object existingSender) &&
                existingSender.ToString() == senderUid);

            if (alreadyRequested)
            {
                Debug.LogWarning("이미 친구 요청을 보냈습니다.");
                return;
            }
        }
        
        var requestData = new Dictionary<string, object>
        {
            { "senderUid", senderUid },
            { "status", "pending" },
            { "timestamp", FieldValue.ServerTimestamp }
        };
        await recipientDoc.UpdateAsync("FriendRequests", FieldValue.ArrayUnion(requestData));
        Debug.Log($"{senderUid}가 {recipientUid}에게 친구 요청을 보냈습니다.");
    }
    // 친구 요청 수락
    public async Task AcceptFriendRequestAsync(string userUid, string requesterUid)
    {
        WriteBatch batch = FirebaseInitialize.DB.StartBatch();

        DocumentReference userDoc = FirebaseInitialize.DB.Collection("Nicknames").Document(userUid);
        DocumentReference requesterDoc = FirebaseInitialize.DB.Collection("Nicknames").Document(requesterUid);

        DocumentSnapshot snapshot = await userDoc.GetSnapshotAsync();

        // 0. 이미 친구인지 확인
        if (snapshot.TryGetValue("Friends", out List<string> friendsList) &&
            friendsList.Contains(requesterUid))
        {
            Debug.LogWarning("이미 친구입니다.");
            return;
        }

        // 1. 서로의 Friends 필드에 추가
        batch.Update(userDoc, new Dictionary<string, object>
        {
            { "Friends", FieldValue.ArrayUnion(requesterUid) }
        });
        batch.Update(requesterDoc, new Dictionary<string, object>
        {
            { "Friends", FieldValue.ArrayUnion(userUid) }
        });

        // 2. 요청 제거 (정확히 일치하는 Dictionary 찾아서 제거)
        if (snapshot.TryGetValue("FriendRequests", out List<Dictionary<string, object>> friendRequests))
        {
            Dictionary<string, object> matchedRequest = friendRequests
                .FirstOrDefault(req => req.TryGetValue("senderUid", out object senderObj) &&
                                       senderObj.ToString() == requesterUid);

            if (matchedRequest != null)
            {
                batch.Update(userDoc, new Dictionary<string, object>
                {
                    { "FriendRequests", FieldValue.ArrayRemove(matchedRequest) }
                });
            }
        }

        await batch.CommitAsync();
        Debug.Log($"{userUid}가 {requesterUid}의 친구 요청을 수락했습니다.");
    }

    // 친구 삭제 
    public async Task RemoveFriendAsync(string userUid, string friendUid)
    {
        WriteBatch batch = FirebaseInitialize.DB.StartBatch();

        DocumentReference userDoc = FirebaseInitialize.DB.Collection("Nicknames").Document(userUid);
        DocumentReference friendDoc = FirebaseInitialize.DB.Collection("Nicknames").Document(friendUid);

        // 친구 삭제전 체크
        DocumentSnapshot snapshot = await userDoc.GetSnapshotAsync();

        if (snapshot.TryGetValue("Friends", out List<string> friendsList))
        {
            if (!friendsList.Contains(friendUid))
            {
                Debug.LogWarning("삭제할 친구가 존재하지 않습니다.");
                return;
            }
        }
        
        // 서로의 Friends 필드에서 UID 제거
        batch.Update(userDoc, new Dictionary<string, object>
        {
            { "Friends", FieldValue.ArrayRemove(friendUid) }
        });
        batch.Update(friendDoc, new Dictionary<string, object>
        {
            { "Friends", FieldValue.ArrayRemove(userUid) }
        });

        await batch.CommitAsync();
        Debug.Log($"{userUid}와 {friendUid}의 친구 관계가 해제되었습니다.");
    }

    // 친구 요청 거절
    public async Task DeclineFriendRequestAsync(string recipientUid, string senderUid)
    {
        DocumentReference recipientDoc = FirebaseInitialize.DB.Collection("Nicknames").Document(recipientUid);
        DocumentSnapshot snapshot = await recipientDoc.GetSnapshotAsync();

        if (snapshot.TryGetValue("FriendRequests", out List<Dictionary<string, object>> friendRequests))
        {
            Dictionary<string, object> matchedRequest = friendRequests
                .FirstOrDefault(req => req.TryGetValue("senderUid", out object senderObj) &&
                                       senderObj.ToString() == senderUid);

            if (matchedRequest != null)
            {
                await recipientDoc.UpdateAsync("FriendRequests", FieldValue.ArrayRemove(matchedRequest));
                Debug.Log($"{recipientUid}가 {senderUid}의 친구 요청을 거절했습니다.");
            }
            else
            {
                Debug.LogWarning("해당 요청을 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("FriendRequests 필드가 없거나 비어 있습니다.");
        }
    }
}
