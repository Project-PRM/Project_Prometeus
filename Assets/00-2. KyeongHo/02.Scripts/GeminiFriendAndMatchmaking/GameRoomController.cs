
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

// 게임 룸 내부의 로직(팀 할당, 게임 시작, 상태 동기화)을 관리하는 클래스입니다.
public class GameRoomController : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // 씬에 컨트롤러가 하나만 있도록 보장합니다.
    }

    // 새로운 플레이어가 룸에 입장했을 때 호출됩니다.
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName}님이 룸에 입장했습니다.");

        // 마스터 클라이언트만 팀 할당 및 룸 상태 업데이트를 처리합니다.
        if (PhotonNetwork.IsMasterClient)
        {
            AssignPlayerToTeam(newPlayer);
            UpdateRoomProperties();
        }
    }

    // 마스터 클라이언트가 플레이어를 팀에 할당하는 로직입니다.
    private void AssignPlayerToTeam(Player player)
    {
        // TODO: 보고서에 기술된 동적 팀 구성 로직 구현
        // 1. 기존 팀들을 순회하며 빈 자리가 있는지 확인합니다.
        // 2. 빈 자리가 있으면 해당 팀에 플레이어를 할당합니다.
        // 3. 빈 자리가 없으면 새로운 팀을 생성하고 플레이어를 할당합니다.
        // 4. 플레이어의 CustomProperties에 TeamID를 설정합니다.

        Hashtable playerProps = new Hashtable { { "TeamID", "TeamA" } }; // 임시 할당
        player.SetCustomProperties(playerProps);
        Debug.Log($"{player.NickName}님을 TeamA에 할당했습니다.");
    }

    // 룸의 사용자 정의 속성(플레이어 수, 팀 수)을 업데이트합니다.
    private void UpdateRoomProperties()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        // TODO: 실제 팀 수를 계산하는 로직 구현
        int teamCount = 1; // 임시 값

        Hashtable roomProps = new Hashtable
        {
            { "PC", playerCount },
            { "TC", teamCount }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
        Debug.Log($"룸 속성 업데이트: 플레이어 수 = {playerCount}, 팀 수 = {teamCount}");
    }

    // 게임 시작 로직 (마스터 클라이언트만 호출 가능)
    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // TODO: 모든 플레이어가 준비되었는지, 5팀 15명이 모두 찼는지 확인하는 로직 추가
            Debug.Log("게임 시작!");

            // 룸을 닫아 더 이상 새로운 플레이어가 들어오지 못하게 합니다.
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            // 모든 클라이언트에게 게임 씬을 로드하도록 명령합니다.
            PhotonNetwork.LoadLevel("GameScene"); // "GameScene"은 실제 게임 씬 이름으로 변경해야 합니다.
        }
    }

    // 마스터 클라이언트가 변경되었을 때 호출됩니다.
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"새로운 마스터 클라이언트: {newMasterClient.NickName}");
        // 새로운 마스터 클라이언트는 즉시 룸 관리 책임을 이어받아야 합니다.
    }

    // 플레이어가 룸을 떠났을 때 호출됩니다.
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName}님이 룸을 떠났습니다.");
        if (PhotonNetwork.IsMasterClient)
        {
            UpdateRoomProperties(); // 룸 상태를 다시 업데이트합니다.
        }
    }
}
