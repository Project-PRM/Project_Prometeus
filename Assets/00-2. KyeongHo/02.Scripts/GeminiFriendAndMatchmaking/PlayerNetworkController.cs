
using UnityEngine;
using Photon.Pun;

// 플레이어의 데이터 동기화(위치, 회전 등) 및 RPC 처리를 담당하는 클래스입니다.
public class PlayerNetworkController : MonoBehaviourPun, IPunObservable
{
    private Vector3 latestPos;
    private Quaternion latestRot;

    void Awake()
    {
        // 자신이 생성한 플레이어가 아닌 경우, 입력을 받지 않도록 처리합니다.
        if (!photonView.IsMine)
        {
            // 예: GetComponent<PlayerController>().enabled = false;
        }
    }

    // PhotonView가 관찰하는 동안 주기적으로 호출되어 데이터를 동기화합니다.
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { 
        if (stream.IsWriting)
        {
            // 자신의 데이터를 다른 클라이언트에게 전송합니다.
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // 다른 클라이언트로부터 데이터를 수신합니다.
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
        }
    }

    void Update()
    {
        // 자신의 플레이어가 아닌 경우, 수신한 데이터로 부드럽게 위치를 보간합니다.
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * 10);
        }
    }

    // [PunRPC] 어트리뷰트를 사용하여 원격 프로시저 호출을 정의할 수 있습니다.
    // 예: 플레이어가 발사하는 이벤트를 모든 클라이언트에게 알립니다.
    [PunRPC]
    public void FireEffect()
    {
        // 모든 클라이언트에서 발사 이펙트를 재생하는 코드를 여기에 작성합니다.
        Debug.Log("Fire effect triggered on all clients.");
    }
}
