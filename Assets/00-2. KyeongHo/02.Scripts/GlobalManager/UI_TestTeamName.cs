using System;
using TMPro;
using UnityEngine;

public class UI_TestTeamName : MonoBehaviour
{
   public TextMeshProUGUI TestTeamNameText;


   private void Start()
   {
      PhotonServerManager.Instance.OnGameStarted += Refresh;
   }
   public void Refresh(string text)
   {
      TestTeamNameText.text = $"TeamName : {text}";
   }
   
}
