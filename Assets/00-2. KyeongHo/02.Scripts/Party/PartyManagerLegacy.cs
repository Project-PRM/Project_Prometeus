using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PartyManagerLegacy : PunSingleton<PartyManagerLegacy>
{
    private const byte MaxPartySize = 3;

    public void CreateMyPartyRoom()
    {
        RoomOptions options = new RoomOptions
        {
            IsVisible = false,
            IsOpen = true,
            MaxPlayers = MaxPartySize,
            PlayerTtl = 60000,
            EmptyRoomTtl = 30000
        };
        PhotonNetwork.CreateRoom(AccountManager.Instance.MyAccount.UserId, options);
    }

    public void InviteFriend(string targetUID)
    {
        // 친구에게 UID 전송은 외부 채널 (Firebase, UI 등) 통해 처리
        Debug.Log($"친구 초대: {targetUID}에게 방 이름 {PhotonNetwork.CurrentRoom.Name} 전달");
    }

    public void JoinParty(string targetUID)
    {
        PhotonNetwork.JoinRoom(targetUID);
    }

    public void MatchFromParty()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        int partySize = PhotonNetwork.CurrentRoom.PlayerCount;
        PhotonNetwork.LeaveRoom();
        MonoBehaviourPunCallbacks mono = PhotonServerManager.Instance;
        mono.StartCoroutine(DelayedMatch(partySize));
    }

    private IEnumerator DelayedMatch(int partySize)
    {
        while (PhotonNetwork.InRoom)
            yield return null;
        PhotonNetwork.JoinRandomRoom(null, (byte)(15 - partySize + 1));
    }
}