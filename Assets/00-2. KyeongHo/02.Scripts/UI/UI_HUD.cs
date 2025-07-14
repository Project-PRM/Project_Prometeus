using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class UI_HUD : MonoBehaviour
{
    public TextMeshProUGUI TeamNameText;

    public void Refresh()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("team", out object teamObj))
        {
            string teamName = teamObj as string;
            TeamNameText.text = $"TeamName : {teamName}";
        }
        else
        {
            TeamNameText.text = $"TeamName : ???";
        }
    }
    private void Start()
    {
        Refresh();
    }

}
