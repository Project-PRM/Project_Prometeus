
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Collections.Generic;

// 파티 단위의 매치메이킹을 관리하는 클래스입니다.
public class PartyMatchmakingManager : PunSingleton<PartyMatchmakingManager>
{
    private FirebaseFirestore db;

    // 파티 리더가 매치메이킹을 시작합니다.
    // partyId: 매치메이킹을 시작할 파티의 ID
    public async void StartPartyMatchmaking(string partyId)
    {
        // 1. Firestore에서 현재 파티 정보를 가져옵니다.
        DocumentSnapshot partySnap = await db.Collection("parties").Document(partyId).GetSnapshotAsync();
        if (!partySnap.Exists)
        {
            Debug.LogError("매치메이킹을 시작할 파티 정보를 찾을 수 없습니다.");
            return;
        }

        List<string> memberIds = partySnap.GetValue<List<string>>("memberIds");
        string[] expectedUsers = memberIds.ToArray();

        Debug.Log($"{memberIds.Count}인 파티 매치메이킹을 시작합니다. 파티원: {string.Join(", ", expectedUsers)}");

        // 2. 파티원들을 위한 슬롯을 예약하며 무작위 룸 참가를 시도합니다.
        PhotonNetwork.JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, null, null, expectedUsers);
    }

    // 무작위 룸 참가가 실패하면, 파티 리더가 새로운 룸을 생성합니다.
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("참여할 방을 찾지 못했습니다. 파티를 위한 새로운 방을 생성합니다.");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 15;
        // ... 기타 룸 옵션 설정
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    // 룸에 성공적으로 참여(또는 생성)했을 때 호출됩니다.
    public override void OnJoinedRoom()
    {
        Debug.Log($"룸 '{PhotonNetwork.CurrentRoom.Name}'에 참여했습니다.");

        // 마스터 클라이언트(파티 리더)만 이 로직을 실행합니다.
        if (PhotonNetwork.IsMasterClient)
        {
            // 3. 성공적으로 룸에 들어온 후, 파티원들이 참여할 수 있도록
            //    Firestore의 파티 정보에 Photon 룸 이름을 기록합니다.
            string partyId = ""; // 현재 파티 ID를 가져오는 로직 필요
            db.Collection("parties").Document(partyId).UpdateAsync("currentPhotonRoomName", PhotonNetwork.CurrentRoom.Name);
        }
    }

    // 다른 파티 멤버들은 Firestore의 'currentPhotonRoomName' 필드의 변경을 감지하고,
    // 해당 룸 이름으로 직접 참여(JoinRoom)해야 합니다.
    // 이 로직은 FirestorePartySystem의 ListenToPartyChanges를 통해 구현될 수 있습니다.
}
