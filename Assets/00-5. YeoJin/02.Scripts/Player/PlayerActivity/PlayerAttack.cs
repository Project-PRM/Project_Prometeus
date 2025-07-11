using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerAttack : PlayerActivity
{
    private CharacterController _controller;

    protected override void Start()
    {
        base.Start();
        _controller = GetComponent<CharacterController>();
    }

    public void OnAttack(InputAction.CallbackContext callback)
    {
        if (!_photonView.IsMine) return;

        if (callback.performed)
        {
            // 일반 공격을 함
            Debug.Log("Normal Attack performed");
            _photonView.RPC(nameof(RPC_NormalAttack), RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_NormalAttack()
    {
        Debug.Log("Normal Attack performed via RPC");
    }
}
