using Photon.Pun;
using UnityEngine;
using Unity.Cinemachine;
using FOW;

[RequireComponent(typeof(PhotonView))]
public class CharacterInGameView : MonoBehaviour
{
    [SerializeField] private UI_NicknameIngame _nickname;

    private CharacterBehaviour _characterBehaviour;
    private PhotonView _photonView;

    private void Awake()
    {
        _characterBehaviour = GetComponent<CharacterBehaviour>();
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (_photonView.IsMine)
        {
            SetupCamera();
        }

        TurnOnRevealer();

        if (_nickname != null)
        {
            _nickname.SetName(_photonView.Owner.NickName);
        }
    }

    private void SetupCamera()
    {
        var cam = FindAnyObjectByType<CinemachineCamera>();
        if (cam != null)
        {
            cam.Follow = transform;
            cam.LookAt = transform;
        }
    }

    private void TurnOnRevealer()
    {
        var revealer = GetComponent<FogOfWarRevealer3D>();
        if (revealer == null) return;

        if (_photonView.IsMine)
        {
            revealer.enabled = true;
            return;
        }

        var myTeam = PhotonServerManager.Instance.GetPlayerTeam(PhotonNetwork.LocalPlayer);
        var team = PhotonServerManager.Instance.GetPlayerTeam(_photonView.Owner);

        if (myTeam == team)
        {
            revealer.enabled = true;
        }
    }
}
