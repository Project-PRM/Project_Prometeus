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
        PartyManager.Instance.OnPartyJoinRoom += PartyJoinRoom;
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
        if (!PartyManager.Instance.IsPartyLeader())
        {
            Debug.LogWarning("파티 리더만 매칭을 시작할 수 있습니다.");
            return;
        }

        // 현재 파티 인원수 확인
        int partySize = GetCurrentPartySize();
        Debug.Log($"[PartyMatchmaking] 파티 인원: {partySize}명으로 매칭을 시작합니다.");

        // 파티 인원수에 따른 방 찾기
        JoinRoomForParty(partySize);
    }
    // 현재 파티 인원수를 확인하는 메서드 (LobbyChatManager에서 구현 필요)
    private int GetCurrentPartySize()
    {
        // 파티 채팅에 참여한 인원수를 반환
        // 이 메서드는 LobbyChatManager에서 구현해야 함
        return PartyManager.Instance.GetPartyMemberCount();
    }
    // 파티 인원수에 맞는 방에 참여
    private void JoinRoomForParty(int partySize)
    {
        // 간단한 방식: 커스텀 프로퍼티 없이 최대 플레이어 수만 지정
        byte expectedMaxPlayers = (byte)Mathf.Max(MaxPlayers, partySize);
    
        bool result = PhotonNetwork.JoinRandomRoom(null, expectedMaxPlayers);

        if (!result)
        {
            Debug.LogError("파티 매칭 요청 실패");
        }
        else
        {
            Debug.Log($"파티 인원 {partySize}명으로 랜덤 룸 참여 시도 (간단 버전)");
        }
    }
    // 솔로 매칭
    public void JoinRandomRoom()
    {
        Debug.Log("[SoloMatchmaking] 개인 매칭 시작");
        PhotonNetwork.JoinRandomRoom();
    }

    // 파티 초대 메시지를 받아서 방 참여
    private void PartyJoinRoom(string roomId)
    {
        Debug.Log($"[PartyInvite] 파티 초대로 방 참여: {roomId}");
        PhotonNetwork.JoinRoom(roomId);
    }

    // 팀 배정 메서드 - 파티원끼리 같은 팀 배정
    public void AssignTeams()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        PhotonPlayer[] players = PhotonNetwork.PlayerList;
        List<PhotonPlayer> assignedPlayers = new List<PhotonPlayer>();
        int currentTeamIndex = 0;
        
        // 파티별로 그룹화
        Dictionary<string, List<PhotonPlayer>> partyGroups = new Dictionary<string, List<PhotonPlayer>>();
        List<PhotonPlayer> soloPlayers = new List<PhotonPlayer>();
        
        // 플레이어를 파티별로 분류
        foreach (PhotonPlayer player in players)
        {
            // 파티 정보가 있는지 확인 (CustomProperties나 다른 방법으로 파티 정보 저장 필요)
            string partyInfo = GetPlayerPartyInfo(player);
            
            if (!string.IsNullOrEmpty(partyInfo))
            {
                if (!partyGroups.ContainsKey(partyInfo))
                    partyGroups[partyInfo] = new List<PhotonPlayer>();
                partyGroups[partyInfo].Add(player);
            }
            else
            {
                soloPlayers.Add(player);
            }
        }
        
        // 파티별로 팀 배정
        foreach (var partyGroup in partyGroups)
        {
            List<PhotonPlayer> partyMembers = partyGroup.Value;
            
            // 파티원이 3명 이하인 경우 같은 팀에 배정
            if (partyMembers.Count <= PLAYERS_PER_TEAM && currentTeamIndex < teamNames.Length)
            {
                foreach (PhotonPlayer player in partyMembers)
                {
                    AssignPlayerToTeam(player, currentTeamIndex);
                    assignedPlayers.Add(player);
                }
                
                Debug.Log($"파티 '{partyGroup.Key}' ({partyMembers.Count}명)를 팀 {teamNames[currentTeamIndex]}에 배정");
                currentTeamIndex++;
            }
            // else
            // {
            //     // 파티원이 3명보다 많은 경우 나누어서 배정
            //     for (int i = 0; i < partyMembers.Count; i++)
            //     {
            //         int teamIndex = currentTeamIndex + (i / PLAYERS_PER_TEAM);
            //         if (teamIndex < teamNames.Length)
            //         {
            //             AssignPlayerToTeam(partyMembers[i], teamIndex);
            //             assignedPlayers.Add(partyMembers[i]);
            //         }
            //     }
            //     currentTeamIndex += Mathf.CeilToInt((float)partyMembers.Count / PLAYERS_PER_TEAM);
            // }
        }
        
        // 남은 솔로 플레이어들을 랜덤하게 섞어서 배정
        for (int i = 0; i < soloPlayers.Count; i++)
        {
            PhotonPlayer temp = soloPlayers[i];
            int randomIndex = UnityEngine.Random.Range(i, soloPlayers.Count);
            soloPlayers[i] = soloPlayers[randomIndex];
            soloPlayers[randomIndex] = temp;
        }
        
        // 남은 자리에 솔로 플레이어들 배정
        foreach (PhotonPlayer player in soloPlayers)
        {
            // 현재 팀에 자리가 있는지 확인
            int playersInCurrentTeam = GetPlayersCountInTeam(currentTeamIndex);
            
            if (playersInCurrentTeam < PLAYERS_PER_TEAM && currentTeamIndex < teamNames.Length)
            {
                AssignPlayerToTeam(player, currentTeamIndex);
            }
            else
            {
                currentTeamIndex++;
                if (currentTeamIndex < teamNames.Length)
                {
                    AssignPlayerToTeam(player, currentTeamIndex);
                }
            }
        }
        
        Invoke(nameof(StartGame), 3f);
    }
    // 플레이어를 특정 팀에 배정하는 헬퍼 메서드
    private void AssignPlayerToTeam(PhotonPlayer player, int teamIndex)
    {
        Hashtable props = new Hashtable();
        props[TEAM_PROPERTY_KEY] = teamNames[teamIndex];
        player.SetCustomProperties(props);
        
        TeamIndex[player.ActorNumber] = teamIndex;
        Debug.Log($"플레이어 {player.NickName}를 팀 {teamNames[teamIndex]}에 배정");
    }
    // 특정 팀의 플레이어 수를 계산하는 헬퍼 메서드
    private int GetPlayersCountInTeam(int teamIndex)
    {
        if (teamIndex >= teamNames.Length) return 0;
    
        int count = 0;
        foreach (var kvp in TeamIndex)
        {
            if (kvp.Value == teamIndex) count++;
        }
        return count;
    }
    // 플레이어의 파티 정보를 가져오는 메서드 (구현 필요)
    private string GetPlayerPartyInfo(PhotonPlayer player)
    {
        // 방법 1: CustomProperties에 파티 정보 저장된 경우
        if (player.CustomProperties.TryGetValue("partyName", out object partyName))
        {
            return partyName.ToString();
        }
    
        return null; // 파티 정보 없음 (솔로 플레이어)
    }

    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PrintAllPlayerTeams();
            PhotonNetwork.LoadLevel(2);
        }
    }
    // 방 입장 시 자신의 파티 정보를 CustomProperties에 저장
    private void SetPartyInfoOnJoinRoom()
    {
        string partyName = PartyManager.Instance.GetCurrentPartyName();
        if (!string.IsNullOrEmpty(partyName))
        {
            Hashtable partyProps = new Hashtable();
            partyProps["partyName"] = partyName;
            PhotonNetwork.LocalPlayer.SetCustomProperties(partyProps);
            Debug.Log($"파티 정보 설정: {partyName}");
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
    
        // 파티 정보를 CustomProperties에 저장
        SetPartyInfoOnJoinRoom();
    
        EventManager.Broadcast(new GameStartEvent(GetPlayerTeam(PhotonNetwork.LocalPlayer)));
    
        // 파티 리더라면 파티원들에게 초대 메시지 전송
        if (PartyManager.Instance.IsPartyLeader())
        {
            string roomId = PhotonNetwork.CurrentRoom.Name;
            PartyManager.Instance.SendPartyInvite(roomId);
            Debug.Log($"[PartyLeader] 파티원들에게 초대 메시지 전송: {roomId}");
        }
    }
    
    public override void OnPlayerEnteredRoom(PhotonPlayer newPlayer)
    {
        Debug.Log($"새로운 플레이어 입장: {newPlayer.NickName}");
        EventManager.Broadcast(new GameStartEvent(GetPlayerTeam(PhotonNetwork.LocalPlayer)));
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"현재 인원: {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}");
            
            // 현재 방의 인원수가 최대 인원수와 같다면
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                Debug.Log("방이 꽉 찼습니다. 팀 배정을 시작합니다.");
                AssignTeams(); // 팀 배정 함수 호출
            }
        }
    }

    public override void OnPlayerLeftRoom(PhotonPlayer otherPlayer)
    {
        Debug.Log($"플레이어 퇴장: {otherPlayer.NickName}");
        EventManager.Broadcast(new GameStartEvent(GetPlayerTeam(PhotonNetwork.LocalPlayer)));
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"랜덤 방 입장 실패: {message}, 새로운 방 생성");
    
        // 파티 매칭 중이었다면 파티 인원수를 고려한 방 생성
        int partySize = 0;
        if (PartyManager.Instance.IsPartyLeader())
        {
            partySize = GetCurrentPartySize();
            Debug.Log($"파티 인원 {partySize}명을 위한 방 생성");
        }
    
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = MaxPlayers,
            IsVisible = true,
            IsOpen = true
        };
    
        // 파티 정보를 방 프로퍼티에 저장 (선택사항)
        if (partySize > 0)
        {
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
                {"partySize", partySize}
            };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "partySize" };
        }
    
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftLobby();
        Debug.Log("방 나감ㅇㅇ");
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("마스터서버 재연결 ㅇㅇㅇㅇ");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"연결 끊김: {cause}");
    }

    #endregion
}