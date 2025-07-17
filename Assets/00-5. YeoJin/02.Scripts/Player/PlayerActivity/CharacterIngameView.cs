using Photon.Pun;
using UnityEngine;
using Unity.Cinemachine;
using FOW;

[RequireComponent(typeof(PhotonView))]
public class CharacterInGameView : MonoBehaviour
{
    [SerializeField] private UI_NicknameIngame _nickname;
    private CharacterBehaviour _characterBehaviour;

    private void Awake()
    {
        _characterBehaviour = GetComponent<CharacterBehaviour>();
    }

    private void Start()
    {
        if (_characterBehaviour.PhotonView.IsMine)
        {
            SetupCamera();
        }

        TurnOnRevealer();

        if (_nickname != null)
        {
            _nickname.SetName(_characterBehaviour.PhotonView.Owner.NickName);
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

        if (_characterBehaviour.PhotonView.IsMine)
        {
            revealer.enabled = true;
            return;
        }

        var myTeam = PhotonServerManager.Instance.GetPlayerTeam(PhotonNetwork.LocalPlayer);
        var team = PhotonServerManager.Instance.GetPlayerTeam(_characterBehaviour.PhotonView.Owner);

        if (myTeam == team)
        {
            revealer.enabled = true;
        }
    }
}
