using System;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;

public class LobbyChatManager : Singleton<LobbyChatManager>, IChatClientListener
{
    public event Action<string> OnPartyJoinRoom;
    private ChatClient chatClient;
    private string currentPartyName;
    private bool isPartyLeader = false;
    private bool isJoiningParty = false; // 파티 참여 중인지 확인용

    [Header("UI Elements")]
    public TMP_InputField messageInputField;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI chatText;

    void Update()
    {
        chatClient?.Service();
    }

    // 채팅 연결 상태 확인
    public bool IsConnected()
    {
        return chatClient != null && chatClient.State == ChatState.ConnectedToFrontEnd;
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
        AddChatMessage("System", $"매치 찾음! 파티원들을 초대합니다.");
        Debug.Log($"[PartyInvite] 초대 메시지 전송: {inviteMessage}");
    }

    // 일반 파티 메시지 전송
    public void SendPartyMessage(string message)
    {
        if (string.IsNullOrEmpty(currentPartyName)) return;
        chatClient.PublishMessage(currentPartyName, message);
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

        if (isPartyLeader)
        {
            SendPartyMessage($"👑 파티 리더가 파티를 떠납니다.");
        }

        chatClient.Unsubscribe(new string[] { currentPartyName });
        currentPartyName = "";
        isPartyLeader = false;
        isJoiningParty = false;
        UpdateStatus("파티를 떠났습니다.");
    }

    #region Chat Callbacks

    public void OnConnected()
    {
        UpdateStatus("✅ 채팅 서버 연결 완료!");
        Debug.Log("채팅 연결 완료");
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            if (results[i] && channels[i] == currentPartyName)
            {
                UpdateStatus($"✅ 파티 '{currentPartyName}' 참여 완료!");
                
                // 파티 참여 완료 후 리더 여부 결정
                // 만약 다른 사용자가 없다면 자동으로 리더가 됨
                if (isJoiningParty)
                {
                    isPartyLeader = true; // 일단 리더로 설정
                    isJoiningParty = false;
                    Debug.Log($"[PartyLeader] {PhotonNetwork.NickName}이(가) 파티 리더 후보가 되었습니다.");
                }
                
                SendPartyMessage($"{PhotonNetwork.NickName} joined the party!");
                AddChatMessage("System", $"파티 참여: {currentPartyName}");
            }
        }
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
                AddChatMessage(sender, message);

                // 초대 메시지 처리
                if (message.StartsWith("!invite "))
                {
                    string roomId = message.Substring(8).Trim();
                    Debug.Log($"[PartyInvite] {sender}로부터 방 초대: {roomId}");
                    OnPartyJoinRoom?.Invoke(roomId);
                    AddChatMessage("System", $"🎮 {sender}님이 매치를 찾았습니다! 참여중...");
                }
                // 다른 사용자의 join 메시지를 받으면 자신은 리더가 아님
                else if (message.Contains("joined the party!") && isPartyLeader)
                {
                    // 이미 다른 사용자가 있다면 리더 권한 포기
                    isPartyLeader = false;
                    Debug.Log($"[PartyLeader] 다른 사용자가 먼저 있었습니다. 리더 권한 포기.");
                }
            }
        }
    }

    public void OnUserSubscribed(string channel, string user)
    {
        AddChatMessage("System", $"👋 {user}님이 파티에 참여했습니다");
        
        Debug.Log($"[OnUserSubscribed] {user}님 참여, 현재 리더: {(isPartyLeader ? PhotonNetwork.NickName : "없음")}");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        AddChatMessage("System", $"👋 {user}님이 파티를 떠났습니다");
        
        // 리더가 떠났다면 다음 사용자가 리더가 됨 (간단한 구현)
        if (!isPartyLeader && !string.IsNullOrEmpty(currentPartyName))
        {
            isPartyLeader = true;
            Debug.Log($"[PartyLeader] {PhotonNetwork.NickName}이(가) 새로운 파티 리더가 되었습니다.");
            SendPartyMessage($"👑 {PhotonNetwork.NickName}님이 새로운 파티 리더입니다.");
        }
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

    // 사용하지 않는 콜백들
    public void OnUnsubscribed(string[] channels) { }
    public void OnPrivateMessage(string sender, object message, string channelName) { }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }
    public void DebugReturn(DebugLevel level, string message) { }

    #endregion

    #region UI Helper Methods

    private void UpdateStatus(string message)
    {
        Debug.Log($"[채팅 상태] {message}");
        if (statusText != null) statusText.text = message;
    }

    private void AddChatMessage(string sender, string message)
    {
        if (chatText != null)
        {
            string timestamp = DateTime.Now.ToString("HH:mm");
            chatText.text += $"[{timestamp}] {sender}: {message}\n";
        }
    }

    #endregion
}