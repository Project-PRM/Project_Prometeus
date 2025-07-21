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

    public int MaxPlayers;
    private const int PLAYERS_PER_TEAM = 3;
    private const string TEAM_PROPERTY_KEY = "team";
    // 팀 구분용 문자열
    private readonly string[] teamNames = { "A", "B", "C", "D", "E" };
    private string _myTeamName =  string.Empty;
    public string MyTeamName => _myTeamName;

    public Dictionary<int, int> TeamIndex = new();
    
    
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

        PartyManager.Instance.CreateMyPartyRoom();
    }
    public override void OnJoinedRoom()
    {
        Debug.Log($"방 입장 완료! 현재 플레이어 수: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
        EventManager.Broadcast(new GameStartEvent(GetPlayerTeam(PhotonNetwork.LocalPlayer)));
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
        Debug.Log("랜덤 방 입장 실패, 새로운 방 생성");
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
