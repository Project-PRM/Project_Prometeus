using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;

public class PartyManager : Singleton<PartyManager>, IChatClientListener
{
    public event Action<string> OnPartyJoinRoom;
    public GameObject FriendInvitePrefab;
    
    private ChatClient chatClient;
    private string currentPartyName;
    private bool isPartyLeader = false;
    private bool isJoiningParty = false; // 파티 참여 중인지 확인용
    private int currentPartyMemberCount = 0; // 파티 인원수 추적
    private HashSet<string> partyMembers = new HashSet<string>(); // 파티원 목록
    private string partyLeaderName = ""; // 파티 리더 이름 저장
    
    void Update()
    {
        chatClient?.Service();
    }

    // Chat 버튼 클릭 시 - 간단화
    public void JoinPartyChat(string partyName)
    {
        StartCoroutine(ConnectAndJoinParty(partyName));
    }

    private IEnumerator ConnectAndJoinParty(string partyName)
    {
        PartyManager.Instance.ForceConnectToChat();
        
        // 채팅 연결 대기
        float timeout = 10f;
        while (timeout > 0)
        {
            if (PartyManager.Instance.IsConnected())
            {
                PartyManager.Instance.JoinParty(partyName);
                yield break;
            }
            
            yield return new WaitForSeconds(0.5f);
            timeout -= 0.5f;
        }
        
        Debug.LogError("채팅 서버 연결 타임아웃!");
    }

    
    // 채팅 연결 상태 확인
    public bool IsConnected()
    {
        return chatClient != null && chatClient.State == ChatState.ConnectedToFrontEnd;
    }

    // 파티 인원수 반환
    public int GetPartyMemberCount()
    {
        return currentPartyMemberCount;
    }

    // 파티원 목록 반환
    public string[] GetPartyMembers()
    {
        return partyMembers.ToArray();
    }

    // 파티 참여
    public void JoinParty(string partyName)
    {
        if (!IsConnected())
        {
            Debug.LogError("채팅 서버가 연결되지 않았습니다.");
            return;
        }

        currentPartyName = partyName;
        isJoiningParty = true;
        isPartyLeader = false; // 일단 리더가 아니라고 설정
        
        chatClient.Subscribe(new string[] { partyName });
        UpdateStatus($"파티 '{partyName}' 참여 중...");
        
        Debug.Log($"[JoinParty] 파티 참여 시작: {partyName}");
    }

    // 파티 리더 여부 확인
    public bool IsPartyLeader() => isPartyLeader;
    public string GetCurrentPartyName() => currentPartyName;

    // 파티 초대 메시지 전송 (방 입장 시 자동 호출)
    public void SendPartyInvite(string roomId)
    {
        if (!IsPartyLeader() || string.IsNullOrEmpty(currentPartyName)) return;
        
        string inviteMessage = $"!invite {roomId}";
        chatClient.PublishMessage(currentPartyName, inviteMessage);
        Debug.Log("매치 찾음! 파티원들을 초대합니다.");
        Debug.Log($"[PartyInvite] 초대 메시지 전송: {inviteMessage}");
    }
    // 친구 초대 기능
    public void SendFriendInvite( string friendUID, string myNickname)
    {
        // 현재 파티가 없다면 파티 생성
        if (string.IsNullOrEmpty(currentPartyName))
        {
            Debug.Log("파티가 없어 새로 생성합니다.");
        
            currentPartyName = AccountManager.Instance.MyAccount.UserId;
            isJoiningParty = true; // 생성자는 자동으로 리더가 됨
            partyLeaderName = PhotonNetwork.NickName; // 미리 리더로 설정
        
            chatClient.Subscribe(new string[] { currentPartyName });
            UpdateStatus($"파티 '{currentPartyName}' 생성 및 초대 중...");
        
            Debug.Log($"[CreateParty] 파티 생성: {currentPartyName}");
        }
        // 현재 파티가 있지만 리더가 아닌 경우
        else if (!IsPartyLeader())
        {
            Debug.Log("파티 리더만 친구를 초대할 수 있습니다.");
            return;
        }

        // 친구에게 개인 메시지로 초대 전송
        string inviteMessage = $"!partyinvite {currentPartyName} {myNickname}";
        chatClient.SendPrivateMessage(friendUID, inviteMessage);
    
        Debug.Log($"[FriendInvite] {friendUID}에게 파티 초대를 보냈습니다.");
    }

    // 강제 채팅 연결
    public void ForceConnectToChat()
    {
        if (IsConnected()) return;

        chatClient?.Disconnect();
        chatClient = new ChatClient(this);

        string appIdChat = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat;
        string nickname = PhotonNetwork.NickName;
        
        var authValues = new AuthenticationValues(nickname);
        bool result = chatClient.Connect(appIdChat, PhotonNetwork.AppVersion, authValues);
        
        if (result)
        {
            UpdateStatus("채팅 서버 연결 중...");
        }
        else
        {
            Debug.LogError("채팅 연결 실패");
        }
    }

    // 파티 떠나기
    public void LeaveParty()
    {
        if (string.IsNullOrEmpty(currentPartyName)) return;
        // 파티원 목록에서 자신 제거
        partyMembers.Remove(PhotonNetwork.NickName);
        currentPartyMemberCount = 0; // 자신이 떠나면 0으로 리셋
        partyMembers.Clear(); // 목록 초기화

        chatClient.Unsubscribe(new string[] { currentPartyName });
        currentPartyName = "";
        isPartyLeader = false;
        isJoiningParty = false;
        partyLeaderName = ""; // 추가
        UpdateStatus("파티를 떠났습니다.");
    }
    public void OnConnected()
    {
        UpdateStatus("✅ 채팅 서버 연결 완료!");
        Debug.Log("채팅 연결 완료");
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        string messageStr = message.ToString();
    
        // 파티 초대 메시지 처리
        if (messageStr.StartsWith("!partyinvite "))
        {
            string[] parts = messageStr.Split(' ');
            if (parts.Length >= 3)
            {
                string partyName = parts[1];
                string inviterNickname = parts[2];
            
                // 초대 UI 생성 (받는 사람에게만)
                if (FriendInvitePrefab != null)
                {
                    GameObject inviteUI = Instantiate(FriendInvitePrefab);
                    var inviteComponent = inviteUI.GetComponent<UI_PartyInvitePopup>();
                    if (inviteComponent != null)
                    {
                        inviteComponent.SetInviteInfo(partyName, sender, inviterNickname);
                    }
                
                    Debug.Log($"[PartyInvite] {inviterNickname}님으로부터 파티 초대를 받았습니다.");
                }
            }
        }
    }
    public void OnSubscribed(string[] channels, bool[] results)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            if (results[i] && channels[i] == currentPartyName)
            {
                // 자신을 파티원에 추가
                partyMembers.Add(PhotonNetwork.NickName);
                currentPartyMemberCount = partyMembers.Count;
                
                UpdateStatus($"✅ 파티 '{currentPartyName}' 참여 완료! ({currentPartyMemberCount}명)");
                
                if (isJoiningParty)
                {
                    isPartyLeader = true; // 일단 리더로 설정
                    partyLeaderName = PhotonNetwork.NickName;
                    isJoiningParty = false;
                    Debug.Log($"[PartyLeader] {PhotonNetwork.NickName}이(가) 파티 리더가 되었습니다.");
                }
                else
                {
                    isPartyLeader = false;
                }
                
                Debug.Log($"파티 참여: {currentPartyName} ({currentPartyMemberCount}명)");
            }
        }
    }
    public void OnUnsubscribed(string[] channels)
    {
        throw new NotImplementedException();
    }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new NotImplementedException();
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (channelName != currentPartyName) return;

        for (int i = 0; i < senders.Length; i++)
        {
            string message = messages[i].ToString();
            string sender = senders[i];

            // 자신이 보낸 메시지가 아닌 경우만 표시
            if (sender != PhotonNetwork.NickName)
            {
                // 초대 메시지 처리
                if (message.StartsWith("!invite "))
                {
                    string roomId = message.Substring(8).Trim();
                    Debug.Log($"[PartyInvite] {sender}로부터 방 초대: {roomId}");
                    OnPartyJoinRoom?.Invoke(roomId);
                    Debug.Log($"🎮 {sender}님이 매치를 찾았습니다! 참여중...");
                }
            }
        }
    }

    public void OnUserSubscribed(string channel, string user)
    {
        if (channel == currentPartyName)
        {
            partyMembers.Add(user);
            currentPartyMemberCount = partyMembers.Count;
            
            Debug.Log($"👋 {user}님이 파티에 참여했습니다 ({currentPartyMemberCount}명)");
            Debug.Log($"[PartyCount] 현재 파티 인원: {currentPartyMemberCount}명");
            
            // UI 업데이트 이벤트 발생 (필요시)
            //EventManager.Broadcast(new PartyMemberCountChangedEvent(currentPartyMemberCount));
            
            // 파티 리더가 떠났다면 랜덤하게 다음 사용자가 리더가 됨
            if (user == partyLeaderName && currentPartyMemberCount > 0)
            {
                // 남은 파티원 중 랜덤하게 새 리더 선택
                var remainingMembers = partyMembers.Where(m => m != PhotonNetwork.NickName).ToArray();
                if (remainingMembers.Length > 0)
                {
                    partyLeaderName = remainingMembers[UnityEngine.Random.Range(0, remainingMembers.Length)];
                }
                else
                {
                    partyLeaderName = PhotonNetwork.NickName; // 자신만 남았다면
                }
            
                if (partyLeaderName == PhotonNetwork.NickName)
                {
                    isPartyLeader = true;
                    Debug.Log($"[PartyLeader] {PhotonNetwork.NickName}이(가) 새로운 파티 리더가 되었습니다.");
                }
            }
        }
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        if (channel == currentPartyName)
        {
            partyMembers.Remove(user);
            currentPartyMemberCount = partyMembers.Count;

            Debug.Log($"👋 {user}님이 파티를 떠났습니다 ({currentPartyMemberCount}명)");
            Debug.Log($"[PartyCount] 현재 파티 인원: {currentPartyMemberCount}명");

            // 리더가 떠났다면 다음 사용자가 리더가 됨
            if (!isPartyLeader && currentPartyMemberCount > 0)
            {
                isPartyLeader = true;
                Debug.Log($"[PartyLeader] {PhotonNetwork.NickName}이(가) 새로운 파티 리더가 되었습니다.");
            }
        }
    }

    public void DebugReturn(DebugLevel level, string message)
    {
    }
    public void OnDisconnected()
    {
        UpdateStatus("❌ 채팅 서버 연결 끊김!");
        Debug.LogWarning("채팅 연결 끊김 - 재연결 시도");
        Invoke(nameof(ForceConnectToChat), 2f);
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log($"채팅 상태 변경: {state}");
    }
    private void UpdateStatus(string message)
    {
        Debug.Log($"[채팅 상태] {message}");
    }
}