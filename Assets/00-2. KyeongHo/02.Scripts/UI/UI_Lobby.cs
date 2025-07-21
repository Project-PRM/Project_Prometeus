using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Lobby : MonoBehaviour
{
   public TextMeshProUGUI TeamNameText;
   public TextMeshProUGUI RoomPlayerCountText;
   public TextMeshProUGUI SystemMessageText;
   
     
   private void Start()
   {
      EventManager.AddListener<GameStartEvent>(Refresh);
   }
   public void Refresh(GameStartEvent evt = null)
   {
      if (evt != null)
      {
         // TeamNameText.text = $"TeamName : {evt.TeamName}";  
         TeamNameText.text = $"TeamName : {PhotonNetwork.NickName} / {PhotonNetwork.LocalPlayer.UserId}" ;  
      }
      RoomPlayerCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
   }

   public void OnClickGameStartButton()
   {
      // 15명이 모두 모이면 게임 시작 (마스터 클라이언트가)
      if(PhotonNetwork.IsMasterClient /*&& PhotonServerManager.Instance.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount*/)
      {
         //방이 꽉차면 더 이상 새로운 플레이어가 들어오지 못함
         PhotonNetwork.CurrentRoom.IsOpen = false;
          
         PhotonServerManager.Instance.AssignTeams();
      }
      else if(!PhotonNetwork.IsMasterClient)
      {
         Debug.Log("방장만 시작 가능");
      }
      else
      {
         Debug.Log($" 방 인원이 부족한데용 {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
         ShowSystemMessage("방 인원이 부족합니다");
      }
   }

   public void OnClickMatchingStartButton()
   {
      TeamNameText.gameObject.SetActive(true);
      RoomPlayerCountText.gameObject.SetActive(true);
      PhotonServerManager.Instance.JoinRandomRoom();
      Debug.Log("매칭 시작...........");
   }
   public void OnClickMatchingStartButtonNew()
   {
      TeamNameText.gameObject.SetActive(true);
      RoomPlayerCountText.gameObject.SetActive(true);

      if (PhotonNetwork.CurrentRoom.Name == AccountManager.Instance.MyAccount.UserId)
      {
         // 파티방이라면 파티 인원 체크 후 매칭
         int partySize = PhotonNetwork.CurrentRoom.PlayerCount;
         if (partySize >= 1 && partySize <= 3)
         {
            Debug.Log($"파티 인원: {partySize}명 → 매칭 시도");
            PartyManager.Instance.MatchFromParty();
         }
         else
         {
            Debug.Log("1~3인 파티만 매칭 가능합니다.");
            ShowSystemMessage("1~3인 파티만 매칭 가능합니다.");
         }
      }
      else
      {
         // 일반 매칭
         PhotonServerManager.Instance.JoinRandomRoom();
      }
   }
   public void OnClickInviteButton(string targetUID)
   {
      PartyManager.Instance.InviteFriend(targetUID);
      ShowSystemMessage($"{targetUID} 에게 초대 전송 완료");
   }
   
   public void ShowSystemMessage(string msg)
   {
      SystemMessageText.gameObject.SetActive(true);
      SystemMessageText.text = msg;
      StartCoroutine(SystemMessageCount());
   }
   IEnumerator SystemMessageCount()
   {
      yield return new WaitForSeconds(3f);
      SystemMessageText.gameObject.SetActive(false);
   }
}
