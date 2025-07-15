using Photon.Pun;
using System;
using UnityEngine;

public class Portal : MonoBehaviour
{
    // 플레이어와 닿으면 게임 승리(임시)
    public Action<GameObject> OnPlayerReachPortal;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 이벤트 호출 : 플레이어와 닿았다 -> 플레이어 넘겨줌(other.gameobject)
        OnPlayerReachPortal?.Invoke(other.gameObject);
    }
}