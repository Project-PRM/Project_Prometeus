
using UnityEngine;
using Firebase;
using Firebase.Database;
using System.Threading.Tasks;
using System.Collections.Generic;

// Firebase Realtime Database를 사용하여 친구 관계 및 요청을 관리하는 클래스입니다.
public class FirebaseFriendSystem : MonoBehaviour
{
    public static FirebaseFriendSystem Instance { get; private set; }

    private DatabaseReference dbRef;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFirebase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Firebase 초기화 및 Database 참조를 설정합니다.
    private void InitializeFirebase()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("Firebase 친구 시스템이 성공적으로 초기화되었습니다.");
    }

    // 다른 사용자에게 친구 요청을 보냅니다.
    public async Task SendFriendRequestAsync(string senderId, string recipientId)
    {
        var requestData = new Dictionary<string, object>
        {
            { "status", "pending" },
            { "timestamp", ServerValue.Timestamp }
        };

        // 트랜잭션을 사용하여 데이터 일관성을 보장합니다.
        // 보낸 사람의 outgoing 목록과 받는 사람의 incoming 목록에 동시에 기록합니다.
        await dbRef.Child("users").Child(senderId).Child("requests").Child("outgoing").Child(recipientId).SetValueAsync(requestData);
        await dbRef.Child("users").Child(recipientId).Child("requests").Child("incoming").Child(senderId).SetValueAsync(requestData);
        Debug.Log($"{senderId}가 {recipientId}에게 친구 요청을 보냈습니다.");
    }

    // 친구 요청을 수락합니다.
    public async Task AcceptFriendRequestAsync(string userId, string requesterId)
    {
        // 양쪽 사용자의 친구 목록에 서로를 추가합니다.
        await dbRef.Child("users").Child(userId).Child("friends").Child(requesterId).SetValueAsync(true);
        await dbRef.Child("users").Child(requesterId).Child("friends").Child(userId).SetValueAsync(true);

        // 요청 목록에서 해당 요청을 제거합니다.
        await dbRef.Child("users").Child(userId).Child("requests").Child("incoming").Child(requesterId).RemoveValueAsync();
        await dbRef.Child("users").Child(requesterId).Child("requests").Child("outgoing").Child(userId).RemoveValueAsync();

        Debug.Log($"{userId}가 {requesterId}의 친구 요청을 수락했습니다.");
    }

    // 친구 요청을 거절합니다.
    public async Task DeclineFriendRequestAsync(string userId, string requesterId)
    {
        // 요청 목록에서 해당 요청을 제거합니다.
        await dbRef.Child("users").Child(userId).Child("requests").Child("incoming").Child(requesterId).RemoveValueAsync();
        await dbRef.Child("users").Child(requesterId).Child("requests").Child("outgoing").Child(userId).RemoveValueAsync();

        Debug.Log($"{userId}가 {requesterId}의 친구 요청을 거절했습니다.");
    }

    // 사용자의 친구 목록을 가져옵니다.
    public async Task<List<string>> GetFriendsAsync(string userId)
    {
        List<string> friendIds = new List<string>();
        DataSnapshot snapshot = await dbRef.Child("users").Child(userId).Child("friends").GetValueAsync();
        if (snapshot.Exists)
        {
            foreach (var childSnapshot in snapshot.Children)
            {
                friendIds.Add(childSnapshot.Key);
            }
        }
        return friendIds;
    }

    // 사용자의 온라인 상태를 업데이트합니다.
    public void SetOnlineStatus(string userId, bool isOnline)
    {
        string status = isOnline ? "online" : "offline";
        dbRef.Child("users").Child(userId).Child("onlineStatus").SetValueAsync(status);
    }
}
