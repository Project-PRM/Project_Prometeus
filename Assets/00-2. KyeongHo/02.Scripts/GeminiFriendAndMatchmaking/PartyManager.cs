
using UnityEngine;
using Firebase.Database;
using System.Collections.Generic;
using System.Threading.Tasks;

// 게임 내 파티 생성, 관리 및 초대를 담당하는 클래스입니다.
public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance { get; private set; }

    private DatabaseReference dbRef;
    public Party CurrentParty { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 새로운 파티를 생성합니다.
    public async Task<string> CreatePartyAsync(string leaderId, int expectedTeamSize)
    {
        string partyId = dbRef.Child("parties").Push().Key;
        Party newParty = new Party
        {
            leaderId = leaderId,
            memberIds = new List<string> { leaderId },
            status = "forming",
            expectedTeamSize = expectedTeamSize
        };

        string json = JsonUtility.ToJson(newParty);
        await dbRef.Child("parties").Child(partyId).SetRawJsonValueAsync(json);
        CurrentParty = newParty;
        Debug.Log($"파티 생성 완료 (ID: {partyId}), 리더: {leaderId}");
        return partyId;
    }

    // 친구를 파티에 초대합니다. (FCM 대신 Realtime DB를 이용한 간소화된 방식)
    public async Task InviteToPartyAsync(string partyId, string senderId, string recipientId)
    {
        // 실제 구현에서는 Cloud Function을 통해 FCM 메시지를 보내야 합니다.
        // 여기서는 Realtime Database에 초대 정보를 기록하는 것으로 대체합니다.
        var inviteData = new Dictionary<string, object>
        {
            { "partyId", partyId },
            { "senderId", senderId },
            { "status", "pending" }
        };
        await dbRef.Child("users").Child(recipientId).Child("party_invites").Child(partyId).SetValueAsync(inviteData);
        Debug.Log($"{senderId}가 {recipientId}를 파티({partyId})에 초대했습니다.");
    }

    // 파티 초대를 수락하고 파티에 참여합니다.
    public async Task AcceptPartyInviteAsync(string partyId, string userId)
    {
        await dbRef.Child("parties").Child(partyId).Child("memberIds").RunTransaction(data =>
        {
            List<object> members = data.Value as List<object>;
            if (members == null)
            {
                members = new List<object>();
            }
            if (!members.Contains(userId))
            {
                members.Add(userId);
            }
            data.Value = members;
            return TransactionResult.Success(data);
        });

        // 초대 정보 삭제
        await dbRef.Child("users").Child(userId).Child("party_invites").Child(partyId).RemoveValueAsync();
        Debug.Log($"{userId}가 파티({partyId})에 참여했습니다.");
    }

    // 파티에서 나갑니다.
    public async Task LeavePartyAsync(string partyId, string userId)
    {
        await dbRef.Child("parties").Child(partyId).Child("memberIds").RunTransaction(data =>
        {
            List<object> members = data.Value as List<object>;
            if (members != null && members.Contains(userId))
            {
                members.Remove(userId);
            }
            data.Value = members;
            return TransactionResult.Success(data);
        });
        Debug.Log($"{userId}가 파티({partyId})에서 나갔습니다.");
    }
}

// 파티 데이터 구조를 정의하는 클래스입니다.
[System.Serializable]
public class Party
{
    public string leaderId;
    public List<string> memberIds;
    public string status;
    public int expectedTeamSize;
    public string currentPhotonRoomName;
}
