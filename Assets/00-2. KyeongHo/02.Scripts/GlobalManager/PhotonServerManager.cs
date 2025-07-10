using System;
using Photon.Pun;
using UnityEngine;

public class PhotonServerManager : PunSingleton<PhotonServerManager>
{
    private readonly string _gameVersion = "1.0.0";

    private void Init()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;
        PhotonNetwork.GameVersion = _gameVersion;
        
        // 방장이 로드한 씬으로 다른 참여자가 똑같이 이동하게끔 동기화 해주는 옵션
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }
    private void Start()
    {
        Init();
    }
}
