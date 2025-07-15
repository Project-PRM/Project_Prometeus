
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// Photon 서버 연결 및 로비 관리를 담당하는 클래스입니다.
public class PhotonConnectionManager : MonoBehaviourPunCallbacks
{
    public static PhotonConnectionManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ConnectToPhoton();
    }

    // Photon 서버에 연결을 시도합니다.
    public void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Photon 서버에 연결을 시도합니다...");
        }
    }

    // 마스터 서버에 성공적으로 연결되었을 때 호출됩니다.
    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon 마스터 서버에 성공적으로 연결되었습니다.");
        // 기본 로비에 자동으로 참여하도록 설정할 수 있습니다.
        PhotonNetwork.JoinLobby();
    }

    // 로비에 성공적으로 참여했을 때 호출됩니다.
    public override void OnJoinedLobby()
    {
        Debug.Log("Photon 로비에 성공적으로 참여했습니다.");
    }

    // Photon 서버와의 연결이 끊겼을 때 호출됩니다.
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Photon 서버와의 연결이 끊겼습니다. 원인: {cause}");
        // 필요하다면 재연결 로직을 여기에 추가할 수 있습니다.
        ConnectToPhoton(); // 예: 자동으로 재연결 시도
    }
}
