using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(CharacterController))]
public class PlayerMove : PlayerActivity
{
    private Vector2 _movement;
    private CharacterController _controller;

    private const float GRAVITY = -9.81f;
    private float _yVelocity = 0;

    protected override void Awake()
    {
        base.Awake();
        _controller = GetComponent<CharacterController>();
    }

    public void OnMove(InputAction.CallbackContext callback)
    {
        if (!_photonView.IsMine) return;

        if (callback.performed || callback.canceled)
        {
            _movement = callback.ReadValue<Vector2>();
        }
    }

    private void Update()
    {
        if (!_photonView.IsMine) return;

        Vector3 move = new Vector3(_movement.x, 0, _movement.y);

        // 회전 (입력 방향이 있을 때만)
        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 30f * Time.deltaTime);
        }

        // 중력
        if (_controller.isGrounded)
        {
            _yVelocity = -1f;
        }
        else
        {
            _yVelocity += GRAVITY * Time.deltaTime;
        }

        move.y = _yVelocity;

        // 이동
        _controller.Move(move * 5f * Time.deltaTime);
    }
}