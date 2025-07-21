using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    // 플레이어와 닿으면 게임 승리(임시)
    public Action<GameObject> OnPlayerReachPortal;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView view = other.GetComponent<PhotonView>();
        if (view == null || !view.IsMine) return;
        PhotonNetwork.AutomaticallySyncScene = false;
        // 이벤트 호출 : 플레이어와 닿았다 -> 플레이어 넘겨줌(other.gameobject)
        OnPlayerReachPortal?.Invoke(other.gameObject);


        Debug.Log($"{other.gameObject.name} (Local) has reached the portal. Loading scene");

        // 개별 씬 전환
        SceneManager.LoadScene(3);
    }
}