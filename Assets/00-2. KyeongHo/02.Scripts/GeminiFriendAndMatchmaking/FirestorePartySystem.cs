
using UnityEngine;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Collections.Generic;

// Firebase Firestore를 사용하여 파티를 생성, 관리하고 초대를 처리하는 클래스입니다.
public class FirestorePartySystem : Singleton<FirestorePartySystem>
{

    private FirebaseFirestore db;
    private ListenerRegistration partyListener;

    // 새로운 파티를 생성합니다.
    // leaderId: 파티를 생성하는 리더의 사용자 ID
    public async Task<string> CreatePartyAsync(string leaderId)
    {
        DocumentReference partyRef = db.Collection("parties").Document();
        var partyData = new Dictionary<string, object>
        {
            { "leaderId", leaderId },
            { "memberIds", new List<string> { leaderId } },
            { "status", "forming" },
            { "createdAt", FieldValue.ServerTimestamp }
        };
        await partyRef.SetAsync(partyData);
        Debug.Log($"파티 생성 완료 (ID: {partyRef.Id}), 리더: {leaderId}");
        return partyRef.Id;
    }

    // 친구를 현재 파티에 초대합니다.
    // partyId: 현재 속한 파티의 ID
    // senderId: 초대를 보내는 사용자 ID
    // recipientId: 초대를 받는 사용자 ID
    public async Task InviteToPartyAsync(string partyId, string senderId, string recipientId)
    {
        DocumentReference inviteRef = db.Collection("partyInvites").Document();
        var inviteData = new Dictionary<string, object>
        {
            { "partyId", partyId },
            { "senderId", senderId },
            { "recipientId", recipientId },
            { "status", "pending" }
        };
        await inviteRef.SetAsync(inviteData);
        Debug.Log($"{senderId}가 {recipientId}를 파티({partyId})에 초대했습니다.");
    }

    // 파티 초대를 수락하고 파티에 참여합니다.
    // inviteId: partyInvites 컬렉션의 초대 문서 ID
    // partyId: 참여할 파티의 ID
    // userId: 파티에 참여할 사용자의 ID
    public async Task AcceptPartyInviteAsync(string inviteId, string partyId, string userId)
    {
        DocumentReference partyRef = db.Collection("parties").Document(partyId);
        DocumentReference inviteRef = db.Collection("partyInvites").Document(inviteId);

        WriteBatch batch = db.StartBatch();
        // 1. 파티의 memberIds 필드에 새로운 사용자를 추가합니다.
        batch.Update(partyRef, "memberIds", FieldValue.ArrayUnion(userId));
        // 2. 처리된 초대 문서를 삭제합니다.
        batch.Delete(inviteRef);

        await batch.CommitAsync();
        Debug.Log($"{userId}가 파티({partyId})에 참여했습니다.");
    }

    // 파티의 정보가 변경될 때 실시간으로 감지하는 리스너를 등록합니다.
    public void ListenToPartyChanges(string partyId, System.Action<DocumentSnapshot> onPartyUpdated)
    {
        partyListener = db.Collection("parties").Document(partyId).Listen(snapshot =>
        {
            if (snapshot.Exists)
            {
                onPartyUpdated(snapshot);
            }
        });
    }

    // 리스너를 제거합니다.
    void OnDestroy()
    {
        partyListener?.Stop();
    }
}
