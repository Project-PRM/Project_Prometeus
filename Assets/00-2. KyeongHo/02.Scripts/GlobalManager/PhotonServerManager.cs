using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonServerManager : PunSingleton<PhotonServerManager>
{
    private readonly string _gameVersion = "1.0.0";
    private readonly TypedLobby _lobbyA = new TypedLobby("A", LobbyType.Default);
    private readonly TypedLobby _lobbyB = new TypedLobby("B", LobbyType.Default);
    
    public int MaxPlayers = 15;
    private const int PLAYERS_PER_TEAM = 3;
    private const string TEAM_PROPERTY_KEY = "team";
    // 팀 구분용 문자열
    private readonly string[] teamNames = { "A", "B", "C", "D", "E" };
    private string _myTeamName =  string.Empty;
    public string MyTeamName => _myTeamName;

    public Dictionary<int, int> TeamIndex = new();
    
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
        // 랜덤 방 입장 실패 시 새로운 방 생성
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
            //Debug.Log("게임 시작! 게임 씬으로 전환합니다.");
            
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

        // 로비 입장 후 자동으로 매치 찾기 시작
    }
    public override void OnJoinedRoom()
    {
        Debug.Log($"방 입장 완료! 현재 플레이어 수: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
        EventManager.Broadcast(new GameStartEvent(GetPlayerTeam(PhotonNetwork.LocalPlayer)));
        
        // OnPlayerEnteredRoom에서 처리하는걸로 수정중
        // if (PhotonNetwork.CurrentRoom.PlayerCount >= MAX_PLAYERS)
        // {
        //     AssignTeams();
        //     StartGame();
        // }
    }
    public override void OnPlayerEnteredRoom(PhotonPlayer newPlayer)
    {
  
        Debug.Log($"새로운 플레이어 입장: {newPlayer.NickName}");
        Debug.Log($"현재 플레이어 수: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
        EventManager.Broadcast(new GameStartEvent(GetPlayerTeam(PhotonNetwork.LocalPlayer)));
    }

    public override void OnPlayerLeftRoom(PhotonPlayer otherPlayer)
    {
        Debug.Log($"플레이어 퇴장: {otherPlayer.NickName}");
        Debug.Log($"현재 플레이어 수: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
        EventManager.Broadcast(new GameStartEvent(GetPlayerTeam(PhotonNetwork.LocalPlayer)));
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"랜덤 방 입장 실패: {message}");
        Debug.Log("새로운 방을 생성합니다.");
        
        // 랜덤 방 입장 실패 시 새로운 방 생성
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = MaxPlayers,
            IsVisible = true,
            IsOpen = true
        };
        
        PhotonNetwork.JoinRandomOrCreateRoom(
            expectedCustomRoomProperties: null,
            expectedMaxPlayers: (byte)MaxPlayers,
            matchingType: MatchmakingMode.FillRoom,
            typedLobby: _lobbyA,
            sqlLobbyFilter: null,
            roomName: null,
            roomOptions: roomOptions,
            expectedUsers: null
        );
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
    /// 더미 데이터용 함수
// #if UNITY_EDITOR
//     // 유니티 에디터에서만 이 메서드가 보이도록 처리
//     [ContextMenu("Test/Start Game with 15 Dummy Players")]
//     private void Test_StartWithDummyPlayers()
//     {
//         if (!PhotonNetwork.InRoom)
//         {
//             Debug.LogError("테스트를 실행하려면 먼저 방에 입장해야 합니다.");
//             return;
//         }
//
//         if (!PhotonNetwork.IsMasterClient)
//         {
//             Debug.LogWarning("마스터 클라이언트만 테스트를 실행할 수 있습니다.");
//             // 마스터가 아니라면 실행하지 않거나, 로직을 마스터에게 보내 실행하도록 구현할 수 있습니다.
//             return;
//         }
//
//         Debug.Log("=============== 더미 플레이어 테스트 시작 ===============");
//
//         // 현재 방의 실제 플레이어들을 가져옵니다.
//         var playerList = new List<PhotonPlayer>(PhotonNetwork.PlayerList);
//         int realPlayerCount = playerList.Count;
//         
//         // 목표 플레이어 수(15명)를 채우기 위해 더미 플레이어 정보를 생성합니다.
//         // Photon의 Player 객체는 직접 생성할 수 없으므로, 현재 플레이어(나 자신)를 복제하여 사용합니다.
//         // 실제 네트워크 플레이어는 아니지만, CustomProperties를 설정하는 로직을 테스트하기엔 충분합니다.
//         PhotonPlayer myPlayer = PhotonNetwork.LocalPlayer;
//         for (int i = 0; i < MAX_PLAYERS - realPlayerCount; i++)
//         {
//             // 이 더미 플레이어는 실제 네트워크에는 존재하지 않습니다.
//             // 단지 AssignTeams 메서드를 테스트하기 위한 데이터 덩어리입니다.
//             playerList.Add(myPlayer); 
//         }
//
//         // 15명의 플레이어(실제 + 더미) 목록으로 팀 배정 로직을 실행합니다.
//         AssignDummyTeamsTest(playerList.ToArray());
//
//         // 게임 시작 로직을 호출합니다.
//         StartGame();
//
//         Debug.Log("=============== 더미 플레이어 테스트 종료 ===============");
//     }
// #endif
}
