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
   public Button GameStartButton;
   public TextMeshProUGUI RoomPlayerCountText;
   
     
   private void Start()
   {
      EventManager.AddListener<GameStartEvent>(Refresh);
   }
   public void Refresh(GameStartEvent evt)
   {
      TeamNameText.text = $"TeamName : {evt.TeamName}";
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
         PhotonNetwork.LoadLevel(GameScene);
      else
      {
         Debug.Log("방장만 시작 가능");
      }
   }
}
