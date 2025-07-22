using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;
using TMPro;

public class LobbyChatManager : Singleton<LobbyChatManager>, IChatClientListener
{
    public event Action<string> OnPartyJoinRoom;
    private ChatClient chatClient;
    private string currentPartyChannelName;
    private string pendingPartyName;

    [Header("UI Elements")]
    public TMP_InputField messageInputField;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI chatText;
    public Button sendMessageButton;

    void Start()
    {
        if (sendMessageButton != null)
            sendMessageButton.onClick.AddListener(SendMessage);
    }

    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }
        
        // Enter키로 메시지 보내기
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendMessage();
        }
    }

    // 외부에서 강제로 채팅 연결을 시작할 수 있는 메서드
    public void ForceConnectToChat()
    {
        Debug.Log("강제 채팅 연결 시도...");
        ConnectToChat();
    }

    // ChatClient 상태를 외부에서 확인할 수 있는 메서드
    public ChatClient GetChatClient()
    {
        return chatClient;
    }

    private void ConnectToChat()
    {
        // 기존 연결이 있다면 정리
        if (chatClient != null)
        {
            if (chatClient.State == ChatState.ConnectedToFrontEnd)
            {
                Debug.Log("이미 채팅 서버에 연결되어 있습니다.");
                return;
            }
            
            chatClient.Disconnect();
            chatClient = null;
        }

        // PhotonNetwork 연결 상태 확인
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            UpdateStatus("❌ Photon Network가 연결되지 않았습니다!");
            Debug.LogError("PhotonNetwork가 연결되지 않은 상태에서 채팅 연결을 시도했습니다.");
            return;
        }

        // 닉네임 확인
        string nickname = PhotonNetwork.NickName;
        if (string.IsNullOrEmpty(nickname))
        {
            UpdateStatus("❌ 닉네임이 설정되지 않았습니다!");
            Debug.LogError("PhotonNetwork.NickName이 비어있습니다.");
            return;
        }

        // AppIdChat 확인
        string appIdChat = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat;
        if (string.IsNullOrEmpty(appIdChat))
        {
            UpdateStatus("❌ AppIdChat이 설정되지 않았습니다!");
            Debug.LogError("AppIdChat이 PhotonServerSettings에 설정되지 않았습니다.");
            Debug.Log("Window > Photon Unity Networking > PUN Setup에서 AppIdChat을 설정해주세요.");
            return;
        }

        // 새로운 채팅 클라이언트 생성 및 연결
        chatClient = new ChatClient(this);
        AuthenticationValues authValues = new AuthenticationValues(nickname);
        
        Debug.Log($"채팅 연결 시도 - AppIdChat: {appIdChat.Substring(0, 8)}..., Nickname: {nickname}");
        
        bool connectResult = chatClient.Connect(appIdChat, PhotonNetwork.AppVersion, authValues);
        
        if (connectResult)
        {
            UpdateStatus("채팅 서버 연결 중...");
            Debug.Log("채팅 연결 요청이 성공적으로 전송되었습니다.");
        }
        else
        {
            UpdateStatus("❌ 채팅 연결 요청 실패!");
            Debug.LogError("chatClient.Connect() 호출이 실패했습니다.");
        }
    }

    public void JoinParty(string partyName)
    {
        if (string.IsNullOrEmpty(partyName))
        {
            UpdateStatus("파티 이름이 필요합니다.");
            return;
        }

        Debug.Log($"JoinParty 호출됨 - 파티명: {partyName}");

        // 채팅 클라이언트 상태 확인
        if (chatClient == null)
        {
            Debug.LogWarning("ChatClient가 null입니다. 대기열에 추가합니다.");
            pendingPartyName = partyName;
            UpdateStatus("채팅 클라이언트가 준비되지 않았습니다. 연결을 기다리는 중...");
            ForceConnectToChat();
            return;
        }

        Debug.Log($"현재 채팅 상태: {chatClient.State}");

        if (chatClient.State != ChatState.ConnectedToFrontEnd)
        {
            pendingPartyName = partyName;
            UpdateStatus($"채팅 서버 연결 대기 중... (상태: {chatClient.State})");
            return;
        }

        // 파티 참여 시도
        this.currentPartyChannelName = partyName;
        bool subscribeResult = this.chatClient.Subscribe(new string[] { this.currentPartyChannelName });
        
        if (subscribeResult)
        {
            UpdateStatus($"파티 '{this.currentPartyChannelName}' 참여 중...");
            Debug.Log($"파티 구독 요청 성공: {this.currentPartyChannelName}");
        }
        else
        {
            UpdateStatus($"❌ 파티 참여 요청 실패!");
            Debug.LogError($"파티 구독 실패: {this.currentPartyChannelName}");
        }
    }

    public void SendMessage()
    {
        if (chatClient == null || string.IsNullOrEmpty(currentPartyChannelName))
        {
            UpdateStatus("파티 채팅에 연결되지 않았습니다.");
            return;
        }

        string message = messageInputField.text;
        if (string.IsNullOrEmpty(message)) return;

        chatClient.PublishMessage(currentPartyChannelName, message);
        AddChatMessage("Me", message);
        messageInputField.text = "";
    }

    public void LeaveParty()
    {
        if (chatClient != null && !string.IsNullOrEmpty(currentPartyChannelName))
        {
            chatClient.PublishMessage(currentPartyChannelName, $"{PhotonNetwork.NickName} has left the party.");
            chatClient.Unsubscribe(new string[] { currentPartyChannelName });
            currentPartyChannelName = "";
            UpdateStatus("파티를 떠났습니다.");
            ClearChat();
        }
    }

    #region Photon Chat Callbacks

    public void OnConnected()
    {
        UpdateStatus("✅ 채팅 서버 연결 완료!");
        Debug.Log("채팅 클라이언트 연결 성공!");
        
        // 대기 중인 파티가 있으면 자동 참여
        if (!string.IsNullOrEmpty(pendingPartyName))
        {
            Debug.Log($"대기 중인 파티 참여: {pendingPartyName}");
            string partyToJoin = pendingPartyName;
            pendingPartyName = "";
            JoinParty(partyToJoin);
        }
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            Debug.Log($"채널 구독 결과 - {channels[i]}: {results[i]}");
            
            if (results[i] && channels[i] == this.currentPartyChannelName)
            {
                UpdateStatus($"✅ 파티 '{this.currentPartyChannelName}' 참여 완료!");
                string joinMessage = $"{PhotonNetwork.NickName} joined the party!";
                this.chatClient.PublishMessage(this.currentPartyChannelName, joinMessage);
                AddChatMessage("System", $"파티 참여: {this.currentPartyChannelName}");
            }
            else if (!results[i])
            {
                UpdateStatus($"❌ 파티 '{channels[i]}' 참여 실패!");
                Debug.LogError($"채널 구독 실패: {channels[i]}");
            }
        }
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (channelName == this.currentPartyChannelName)
        {
            for (int i = 0; i < senders.Length; i++)
            {
                string message = messages[i].ToString();
                if (senders[i] != PhotonNetwork.NickName) // 자신의 메시지가 아닐 때만 추가
                {
                    AddChatMessage(senders[i], message);
                }
            }
            //명령어 파싱 -> PhotonServerManager에서 Action 구독
            for (int i = 0; i < messages.Length; i++)
            {
                string message = messages[i].ToString();
        
                // 명령어 파싱
                if (message.StartsWith("!invite"))
                {
                    if (message.Length > 7)
                    {
                        string roomId = message.Substring(7);
                        OnPartyJoinRoom?.Invoke(roomId);
                    }
                }
            }
        }
    }

    public void OnDisconnected()
    {
        UpdateStatus("❌ 채팅 서버 연결 끊김!");
        Debug.LogWarning("채팅 서버 연결이 끊어졌습니다. 재연결을 시도합니다.");
        Invoke(nameof(ConnectToChat), 2f);
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log($"채팅 상태 변경: {state}");
        
        if (state == ChatState.Disconnected)
        {
            Debug.LogWarning("채팅 상태가 Disconnected로 변경되었습니다.");
            Invoke(nameof(ConnectToChat), 2f);
        }
    }

    public void OnUnsubscribed(string[] channels) 
    {
        Debug.Log($"채널 구독 해제: {string.Join(", ", channels)}");
    }
    
    public void OnPrivateMessage(string sender, object message, string channelName) { }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }
    public void OnUserSubscribed(string channel, string user) 
    {
        AddChatMessage("System", $"👋 {user}님이 파티에 참여했습니다");
        Debug.Log($"{user}가 채널 {channel}에 참여했습니다.");
    }
    public void OnUserUnsubscribed(string channel, string user) 
    {
        AddChatMessage("System", $"👋 {user}님이 파티를 떠났습니다");
        Debug.Log($"{user}가 채널 {channel}을 떠났습니다.");
    }
    public void DebugReturn(DebugLevel level, string message) 
    {
        Debug.Log($"[Photon Chat Debug - {level}] {message}");
    }

    #endregion

    #region Helper Methods

    private void UpdateStatus(string message)
    {
        Debug.Log($"[채팅 상태] {message}");
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    private void AddChatMessage(string sender, string message)
    {
        if (chatText != null)
        {
            string timestamp = DateTime.Now.ToString("HH:mm");
            chatText.text += $"[{timestamp}] {sender}: {message}\n";
            Canvas.ForceUpdateCanvases();
        }
    }

    private void ClearChat()
    {
        if (chatText != null)
        {
            chatText.text = "";
        }
    }

    #endregion
}