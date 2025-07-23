using Photon.Pun;
using UnityEngine;
using Unity.Cinemachine;
using FOW;
using UnityEngine.TextCore.Text;
using WebSocketSharp;

[RequireComponent(typeof(PhotonView))]
public class CharacterInGameView : MonoBehaviour, IPunObservable
{
    [SerializeField] private UI_NicknameIngame _nickname;
    [SerializeField] private UI_HealthBar _healthBar;
    private CharacterBehaviour _characterBehaviour;
    private CharacterBase _character;

    private float _networkHealthRatio = 1f;

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

        _healthBar.SetValue(1); // 초기값 세팅
    }

    public void OnTakenDamage(ECharacterEvent characterEvent)
    {
        if (_character == null)
            _character = _characterBehaviour.GetCharacterBase();

        if (characterEvent == ECharacterEvent.OnDamaged || characterEvent == ECharacterEvent.OnDeath)
        {
            float healthRatio = _character.CurrentHealth / _character.BaseStats.MaxHealth;


            UpdateHealthBarRPC(healthRatio);
            // 내 캐릭터인 경우에만 RPC 호출
            if (_characterBehaviour.PhotonView.IsMine)
            {
                // RPC로 모든 클라이언트에 체력바 업데이트 전송
                //_characterBehaviour.PhotonView.RPC("UpdateHealthBarRPC", RpcTarget.All, healthRatio);
            }
        }
    }

    void UpdateHealthBarRPC(float healthRatio)
    {
        _networkHealthRatio = healthRatio;
        _healthBar.SetValue(healthRatio);
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 데이터 전송 (마스터가 보내는 경우)
            if (_character != null)
            {
                float healthRatio = _character.CurrentHealth / _character.BaseStats.MaxHealth;
                stream.SendNext(healthRatio);
            }
            else
            {
                stream.SendNext(_networkHealthRatio);
            }
        }
        else
        {
            // 데이터 수신
            _networkHealthRatio = (float)stream.ReceiveNext();

            // 내 캐릭터가 아닌 경우에만 체력바 업데이트
            if (!_characterBehaviour.PhotonView.IsMine)
            {
                _healthBar.SetValue(_networkHealthRatio);
            }
        }
    }
}
