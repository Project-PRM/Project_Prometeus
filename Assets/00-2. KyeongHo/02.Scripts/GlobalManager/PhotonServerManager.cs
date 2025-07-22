using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using AuthenticationValues = Photon.Realtime.AuthenticationValues;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonServerManager : PunSingleton<PhotonServerManager>
{
    private readonly string _gameVersion = "1.0.0";

    public int MaxPlayers;
    private const int PLAYERS_PER_TEAM = 3;
    private const string TEAM_PROPERTY_KEY = "team";
    private readonly string[] teamNames = { "A", "B", "C", "D", "E" };
    private string _myTeamName = string.Empty;
    public string MyTeamName => _myTeamName;

    public Dictionary<int, int> TeamIndex = new();

    protected override void Awake()
    {
        base.Awake();
        LobbyChatManager.Instance.OnPartyJoinRoom += PartyJoinRoom;
    }

    private void Start()
    {
        Init();
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

    // 파티 매칭 시작 (파티 리더만 호출)
    public void StartPartyMatchmaking()
    {
        if (!LobbyChatManager.Instance.IsPartyLeader())
        {
            Debug.LogWarning("파티 리더만 매칭을 시작할 수 있습니다.");
            return;
        }

        Debug.Log("[PartyMatchmaking] 파티 리더가 매칭을 시작합니다.");
        PhotonNetwork.JoinRandomRoom();
    }

    // 솔로 매칭
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    // 파티 초대 메시지를 받아서 방 참여
    private void PartyJoinRoom(string roomId)
    {
        Debug.Log($"[PartyInvite] 파티 초대로 방 참여: {roomId}");
        PhotonNetwork.JoinRoom(roomId);
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
        
        Invoke(nameof(StartGame), 3f);
    }

    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PrintAllPlayerTeams();
            PhotonNetwork.LoadLevel(2);
        }
    }

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

    public void PrintAllPlayerTeams()
    {
        foreach (PhotonPlayer player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"플레이어: {player.NickName}, 팀: {GetPlayerTeam(player)}");
        }
    }

    // Chat 버튼 클릭 시 - 간단화
    public void JoinPartyChat(string partyName)
    {
        StartCoroutine(ConnectAndJoinParty(partyName));
    }

    private IEnumerator ConnectAndJoinParty(string partyName)
    {
        LobbyChatManager.Instance.ForceConnectToChat();
        
        // 채팅 연결 대기
        float timeout = 10f;
        while (timeout > 0)
        {
            if (LobbyChatManager.Instance.IsConnected())
            {
                LobbyChatManager.Instance.JoinParty(partyName);
                yield break;
            }
            
            yield return new WaitForSeconds(0.5f);
            timeout -= 0.5f;
        }
        
        Debug.LogError("채팅 서버 연결 타임아웃!");
    }

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버 접속 완료");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장 완료!");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"방 입장 완료! 현재 플레이어 수: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
        EventManager.Broadcast(new GameStartEvent(GetPlayerTeam(PhotonNetwork.LocalPlayer)));
        
        // 파티 리더라면 파티원들에게 초대 메시지 전송
        if (LobbyChatManager.Instance.IsPartyLeader())
        {
            string roomId = PhotonNetwork.CurrentRoom.Name;
            LobbyChatManager.Instance.SendPartyInvite(roomId);
            Debug.Log($"[PartyLeader] 파티원들에게 초대 메시지 전송: {roomId}");
        }
    }
    
    public override void OnPlayerEnteredRoom(PhotonPlayer newPlayer)
    {
        Debug.Log($"새로운 플레이어 입장: {newPlayer.NickName}");
        EventManager.Broadcast(new GameStartEvent(GetPlayerTeam(PhotonNetwork.LocalPlayer)));
    }

    public override void OnPlayerLeftRoom(PhotonPlayer otherPlayer)
    {
        Debug.Log($"플레이어 퇴장: {otherPlayer.NickName}");
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

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"연결 끊김: {cause}");
    }

    #endregion
}