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
   [Header("GameScene 이름")]
   public string GameScene;
   
   public TextMeshProUGUI TeamNameText;
   public TextMeshProUGUI RoomPlayerCountText;
   public TextMeshProUGUI SystemMessageText;
   public UI_FriendList FriendListPopup;
   public Button GameStartButton;
   public Button MatchingStartButton;
   
     
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
      if (string.IsNullOrEmpty(GameScene))
      {
         Debug.Log("Scene이름이 비어있음");
      }
      // TODO : 지금은 그냥 GameScene으로 넘어가지만, 나중엔 인원이 꽉 차면 알아서 게임 시작
      
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
   public void OnClickFriendsPopupButton()
   {
      FriendListPopup.Show();
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
