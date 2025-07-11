using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonServerManager : PunSingleton<PhotonServerManager>
{
    private readonly string _gameVersion = "1.0.0";
    private readonly TypedLobby _lobbyA = new TypedLobby("A", LobbyType.Default);
    private readonly TypedLobby _lobbyB = new TypedLobby("B", LobbyType.Default);
    
    private const int MAX_PLAYERS = 15;
    private const int PLAYERS_PER_TEAM = 3;
    private const string TEAM_PROPERTY_KEY = "team";
    // 팀 구분용 문자열
    private readonly string[] teamNames = { "A", "B", "C", "D", "E" };
    private string _myTeamName =  string.Empty;
    public event Action<string> OnGameStarted;
    private void Init()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;
        PhotonNetwork.GameVersion = _gameVersion;
        
        // 방장이 로드한 씬으로 다른 참여자가 똑같이 이동하게끔 동기화 해주는 옵션
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = AccountManager.Instance.MyAccount.UserId;
        
        Debug.Log($"Photon UserId : {PhotonNetwork.AuthValues.UserId}");
        PhotonNetwork.ConnectUsingSettings();
    }
    private void Start()
    {
        Init();
    }
    // 솔로큐 매치 찾기 - 15인 랜덤 룸 생성 또는 참여
    public void CreateOrJoinRandomRoom()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = MAX_PLAYERS,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.JoinRandomOrCreateRoom(
            expectedCustomRoomProperties: null,
            expectedMaxPlayers: MAX_PLAYERS,
            matchingType: MatchmakingMode.FillRoom,
            typedLobby: _lobbyA,
            sqlLobbyFilter: null,
            roomName: null,
            roomOptions: roomOptions,
            expectedUsers: null
        );
    }
    // 팀 배정 메서드
    private void AssignTeams()
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
                
                Debug.Log($"플레이어 {players[i].NickName}를 팀 {teamNames[teamIndex]}에 배정");
            }
        }
    }
    // 게임 시작 (씬 전환)
    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("게임 시작! 게임 씬으로 전환합니다.");
            PrintAllPlayerTeams();
            // PhotonNetwork.LoadLevel("GameScene");
        }
        string team;
        PhotonPlayer player = PhotonNetwork.LocalPlayer;
        team = GetPlayerTeam(player);
        OnGameStarted?.Invoke(team);
        
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
        CreateOrJoinRandomRoom();
    }
    public override void OnJoinedRoom()
    {
        Debug.Log($"방 입장 완료! 현재 플레이어 수: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
        
        // 15명이 모두 모이면 게임 시작
        if (PhotonNetwork.CurrentRoom.PlayerCount >= MAX_PLAYERS)
        {
            AssignTeams();
            StartGame();
        }
    }
    public override void OnPlayerEnteredRoom(PhotonPlayer newPlayer)
    {
        Debug.Log($"새로운 플레이어 입장: {newPlayer.NickName}");
        Debug.Log($"현재 플레이어 수: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
        
        // 15명이 모두 모이면 게임 시작
        if (PhotonNetwork.CurrentRoom.PlayerCount >= MAX_PLAYERS)
        {
            AssignTeams();
            Invoke("StartGame", 2f);
            // StartGame();
        }
    }
    public override void OnPlayerLeftRoom(PhotonPlayer otherPlayer)
    {
        Debug.Log($"플레이어 퇴장: {otherPlayer.NickName}");
        Debug.Log($"현재 플레이어 수: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"랜덤 방 입장 실패: {message}");
        Debug.Log("새로운 방을 생성합니다.");
        
        // 랜덤 방 입장 실패 시 새로운 방 생성
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = MAX_PLAYERS,
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
