using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;
using AuthenticationValues = Photon.Realtime.AuthenticationValues;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonServerManager : PunSingleton<PhotonServerManager>
{
    private readonly string _gameVersion = "1.0.0";

    public int MaxPlayers;
    private const int PLAYERS_PER_TEAM = 3;
    private const string TEAM_PROPERTY_KEY = "team";
    // 팀 구분용 문자열
    private readonly string[] teamNames = { "A", "B", "C", "D", "E" };
    private string _myTeamName =  string.Empty;
    public string MyTeamName => _myTeamName;

    public Dictionary<int, int> TeamIndex = new();

    protected override void Awake()
    {
        base.Awake();
        LobbyChatManager.Instance.OnPartyJoinRoom += PartyJoinRoom;
    }
    
    public void StartMatchingFromParty()
    {
        int partySize = PhotonNetwork.CurrentRoom.PlayerCount;
        PhotonNetwork.LeaveRoom();
        StartCoroutine(DelayedMatching(partySize));
    }

    private IEnumerator DelayedMatching(int partySize)
    {
        while (PhotonNetwork.InRoom)
            yield return null;

        PhotonNetwork.JoinRandomRoom(null, (byte)(15 - partySize + 1)); // 최소 partySize 이상 빈 슬롯 필요
    }

    private void PartyJoinRoom(string roomId)
    {
        Debug.Log($"PartyJoinRoom({roomId})호출ㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇㅇ");
        PhotonNetwork.JoinRoom(roomId);
        
    }
    
    private void Init()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;
        PhotonNetwork.GameVersion = _gameVersion;
        PhotonNetwork.AutomaticallySyncScene = true;

        string userId = AccountManager.Instance.MyAccount.UserId;
        string nickname = AccountManager.Instance.MyAccount.Nickname;

        PhotonNetwork.AuthValues = new AuthenticationValues(userId);
        PhotonNetwork.NickName = nickname;

        Debug.Log($"[Init] Photon UserId: {userId}, Nickname: {nickname}");
        PhotonNetwork.ConnectUsingSettings();
    }
    private void Start()
    {
        Init();
    }
    // 솔로큐 매치 찾기 - 15인 랜덤 룸 생성 또는 참여
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    // 팀 배정 메서드
    public void AssignTeams()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        PhotonPlayer[] players = PhotonNetwork.PlayerList;
        
        // 플레이어 목록을 섞기 (랜덤 팀 배정)
        for (int i = 0; i < players.Length; i++)
        {
            PhotonPlayer temp = players[i];
            int randomIndex = UnityEngine.Random.Range(i, players.Length);
            players[i] = players[randomIndex];
            players[randomIndex] = temp;
        }
        
        // 3명씩 팀 배정
        for (int i = 0; i < players.Length; i++)
        {
            int teamIndex = i / PLAYERS_PER_TEAM;
            if (teamIndex < teamNames.Length)
            {
                Hashtable props = new Hashtable();
                props[TEAM_PROPERTY_KEY] = teamNames[teamIndex];
                players[i].SetCustomProperties(props);
                
                TeamIndex[players[i].ActorNumber] = teamIndex;
                Debug.Log($"플레이어 {players[i].NickName}를 팀 {teamNames[teamIndex]}에 배정");
            }
        }
        //팀이 전원 불러와지지 않은 채 시작할 수 있어서 임의로 2초 줌
        Invoke(nameof(StartGame), 3f);
        
    }

    // 게임 시작 (씬 전환)
    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PrintAllPlayerTeams();
            PhotonNetwork.LoadLevel(2);
        }
    }
    // 현재 플레이어의 팀 정보를 가져오는 메서드
    public string GetPlayerTeam(PhotonPlayer player)
    {
        if (player.CustomProperties.TryGetValue(TEAM_PROPERTY_KEY, out object team))
        {
            return team.ToString();
        }
        return "None";
    }
    public override void OnPlayerPropertiesUpdate(PhotonPlayer targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer.IsLocal && changedProps.ContainsKey("team"))
        {
            _myTeamName = changedProps["team"] as string;
            EventManager.Broadcast(new GameStartEvent(GetPlayerTeam(PhotonNetwork.LocalPlayer)));
            Debug.Log($"[LocalPlayer] 팀 이름 갱신됨: {_myTeamName}");
        }
    }

    // 모든 플레이어의 팀 정보를 출력하는 메서드 (디버깅용)
    public void PrintAllPlayerTeams()
    {
        foreach (PhotonPlayer player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"플레이어: {player.NickName}, 팀: {GetPlayerTeam(player)}");
        }
    }
    
    // 친구 초대시 호출    
    public void JoinRoom(string roomName) 
    {
        PhotonNetwork.JoinRoom(roomName);    
    }
    
    /// 이벤트들
    public override void OnConnected()
    {
        Debug.Log("네임 서버 접속완료");
        Debug.Log($"{PhotonNetwork.CloudRegion}");
    }
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버 접속 완료");
        Debug.Log(PhotonNetwork.CloudRegion);
        Debug.Log($"InLobby : {PhotonNetwork.InLobby}");
        
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비(채널) 입장 완료!");
        Debug.Log($"InLobby : {PhotonNetwork.InLobby}");
        // 채팅 매니저 강제 초기화 및 연결
    }

    public void OnChat1ButtonClick()
    {
        StartCoroutine(InitializeChatAndJoinParty("Chat1"));
    }
    public void OnChat2ButtonClick()
    {
        StartCoroutine(InitializeChatAndJoinParty("Chat2"));
    }
    private IEnumerator InitializeChatAndJoinParty(string partyName)
    {
        Debug.Log("채팅 연결 초기화 시작...");
        
        // 1. 채팅 매니저가 존재하는지 확인
        if (LobbyChatManager.Instance == null)
        {
            Debug.LogError("LobbyChatManager.Instance가 null입니다!");
            yield break;
        }

        // 2. 강제로 채팅 연결 시도
        LobbyChatManager.Instance.ForceConnectToChat();
        
        // 3. 채팅 클라이언트 연결 대기 (더 긴 타임아웃과 더 자세한 로그)
        float timeout = 20f; // 20초로 늘림
        float checkInterval = 0.5f; // 체크 간격을 0.5초로 늘림
        
        while (timeout > 0)
        {
            // 현재 채팅 상태 로그
            var chatClient = LobbyChatManager.Instance.GetChatClient();
            if (chatClient != null)
            {
                Debug.Log($"채팅 상태: {chatClient.State}");
                
                if (chatClient.State == ChatState.ConnectedToFrontEnd)
                {
                    Debug.Log("✅ 채팅 서버 연결 완료! 파티 참여 시도...");
                    LobbyChatManager.Instance.JoinParty(partyName);
                    yield break;
                }
                else if (chatClient.State == ChatState.Disconnected)
                {
                    Debug.LogWarning("채팅 연결이 끊어졌습니다. 재연결 시도...");
                    LobbyChatManager.Instance.ForceConnectToChat();
                }
            }
            else
            {
                Debug.LogWarning("ChatClient가 null입니다.");
            }
            
            yield return new WaitForSeconds(checkInterval);
            timeout -= checkInterval;
        }
        
        Debug.LogError($"채팅 서버 연결 타임아웃! (20초) - 현재 상태: {LobbyChatManager.Instance.GetChatClient()?.State}");
        
        // 타임아웃 후에도 재시도
        Debug.Log("5초 후 재시도합니다...");
        yield return new WaitForSeconds(5f);
        StartCoroutine(InitializeChatAndJoinParty(partyName));
    }
    public override void OnJoinedRoom()
    {
        Debug.Log($"방 입장 완료! 현재 플레이어 수: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
        EventManager.Broadcast(new GameStartEvent(GetPlayerTeam(PhotonNetwork.LocalPlayer)));
        
        Debug.Log($"=== 방 참가 완료 ===");
        Debug.Log($"방 이름: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"현재 플레이어 수: {PhotonNetwork.CurrentRoom.PlayerCount}");
        
        // 방에 있는 모든 플레이어 로그
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            Debug.Log($"플레이어: {player.NickName}, UserID: {player.UserId}");
        }
        
        // 방법 1: 직접 계산
        int availableSlots = PhotonNetwork.CurrentRoom.MaxPlayers - PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log($"현재 들어올 수 있는 플레이어 수: {availableSlots}명");

// 방법 2: 더 상세한 정보
        Debug.Log($"최대 플레이어: {PhotonNetwork.CurrentRoom.MaxPlayers}");
        Debug.Log($"현재 플레이어: {PhotonNetwork.CurrentRoom.PlayerCount}"); 
        Debug.Log($"빈 슬롯: {PhotonNetwork.CurrentRoom.MaxPlayers - PhotonNetwork.CurrentRoom.PlayerCount}");
        // 내가 원하는 UID들이 있는지 체크
        CheckTargetUsers();
    }
    private void CheckTargetUsers()
    {
        string[] targetUIDs = {"DoImcscNSyNQ2tgfZ9nuhGQOwqn1", "giiieCwvE3Zk53fGCCLTH7BbT4B2"};
        
        Debug.Log("=== 타겟 UID 체크 ===");
        foreach (string uid in targetUIDs)
        {
            bool found = false;
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if (player.UserId == uid)
                {
                    Debug.Log($"✅ 찾음: {uid} - {player.NickName}");
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Debug.Log($"❌ 없음: {uid}");
            }
        }
        
        // 둘 다 있으면 RPC 테스트
        if (AreAllTargetUsersPresent())
        {
            Debug.Log("🎉 타겟 유저들이 모두 있음! RPC 테스트 호출");
            photonView.RPC("TestRPC", RpcTarget.All, "모든 타겟 유저 접속 완료!");
        }
    }

    private bool AreAllTargetUsersPresent()
    {
        string[] targetUIDs = {"DoImcscNSyNQ2tgfZ9nuhGQOwqn1", "giiieCwvE3Zk53fGCCLTH7BbT4B2"};
        
        foreach (string uid in targetUIDs)
        {
            bool found = false;
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if (player.UserId == uid)
                {
                    found = true;
                    break;
                }
            }
            if (!found) return false;
        }
        return true;
    }

    [PunRPC]
    void TestRPC(string message)
    {
        Debug.Log($"🔥 RPC 받음: {message} (받은 플레이어: {PhotonNetwork.LocalPlayer.UserId})");
    }
    
    public override void OnPlayerEnteredRoom(PhotonPlayer newPlayer)
    {
  
        Debug.Log($"새로운 플레이어 입장: {newPlayer.NickName}");
        Debug.Log($"현재 플레이어 수: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
        EventManager.Broadcast(new GameStartEvent(GetPlayerTeam(PhotonNetwork.LocalPlayer)));
        
        string[] allowedUsers = {"DoImcscNSyNQ2tgfZ9nuhGQOwqn1", "giiieCwvE3Zk53fGCCLTH7BbT4B2"};
    }

    public override void OnPlayerLeftRoom(PhotonPlayer otherPlayer)
    {
        Debug.Log($"플레이어 퇴장: {otherPlayer.NickName}");
        Debug.Log($"현재 플레이어 수: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
        EventManager.Broadcast(new GameStartEvent(GetPlayerTeam(PhotonNetwork.LocalPlayer)));
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("랜덤 방 입장 실패, 새로운 방 생성");
        Debug.LogError($"JoinRandomRoom 실패: {message} (코드: {returnCode})");
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = MaxPlayers,
            IsVisible = true,
            IsOpen = true
        };
        PhotonNetwork.CreateRoom(null, roomOptions);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"방 생성 실패: {message}");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("방에서 나왔습니다.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"연결 끊김: {cause}");
    }
}
