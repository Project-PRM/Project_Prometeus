
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

// 매치메이킹 로직(방 찾기, 생성, 참여)을 관리하는 클래스입니다.
public class MatchmakingManager : MonoBehaviourPunCallbacks
{
    public static MatchmakingManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 1인 플레이어 매치메이킹을 시작합니다.
    public void FindMatchForSolo()
    {
        Debug.Log("1인 매치메이킹을 시작합니다...");
        PhotonNetwork.JoinRandomRoom();
    }

    // 파티(2인 또는 3인) 매치메이킹을 시작합니다.
    public void FindMatchForParty(List<string> partyMemberIds)
    {
        Debug.Log($"{partyMemberIds.Count}인 파티 매치메이킹을 시작합니다...");
        string[] expectedUsers = partyMemberIds.ToArray();

        // 파티 멤버를 위한 슬롯을 예약하며 방을 찾습니다.
        PhotonNetwork.JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, null, null, expectedUsers);
    }

    // 무작위 방 입장에 실패했을 때 호출됩니다. (적절한 방이 없을 경우)
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("참여할 방을 찾지 못했습니다. 새로운 방을 생성합니다.");
        CreateRoom();
    }

    // 새로운 게임 룸을 생성합니다.
    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 15; // 최대 15명
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        // 로비에서 필터링할 사용자 정의 속성을 설정합니다.
        roomOptions.CustomRoomProperties = new Hashtable
        {
            { "PC", 1 }, // PlayerCount
            { "TC", 1 }, // TeamCount
            { "RM", "Matchmaking" } // RoomMode
        };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "PC", "TC", "RM" };

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    // 방에 성공적으로 참여했을 때 호출됩니다.
    public override void OnJoinedRoom()
    {
        Debug.Log($"방 '{PhotonNetwork.CurrentRoom.Name}'에 성공적으로 참여했습니다. 현재 인원: {PhotonNetwork.CurrentRoom.PlayerCount}");
        // 파티 리더인 경우, 다른 파티원들에게 방 이름을 알려 참여하도록 해야 합니다.
        // (Firebase Realtime DB 또는 다른 방법을 통해 구현)
    }

    // 방 생성을 실패했을 때 호출됩니다.
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"방 생성 실패: {message}");
    }
}
