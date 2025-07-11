using System;
using Photon.Pun;
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
         TeamNameText.text = $"TeamName : {evt.TeamName}";  
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
      
      if(PhotonNetwork.IsMasterClient)
         PhotonNetwork.LoadLevel(2);
      else
      {
         Debug.Log("방장만 시작 가능");
      }
   }

   public void OnClickMatchingStartButton()
   {
      TeamNameText.gameObject.SetActive(true);
      RoomPlayerCountText.gameObject.SetActive(true);
      PhotonServerManager.Instance.CreateOrJoinRandomRoom();
      Refresh();
      Debug.Log("매칭 시작...........");
      
   }
}
