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

    private bool _isSprinting = false;

    private Camera _mainCamera;

    protected override void Start()
    {
        base.Start();
        _controller = GetComponent<CharacterController>();
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public void OnMove(InputAction.CallbackContext callback)
    {
        if (!_photonView.IsMine) return;

        if (callback.performed || callback.canceled)
        {
            Vector2 input = callback.ReadValue<Vector2>();
            _movement = input;
            _owner.Animator.SetFloat("Move", _movement.magnitude);
        }
    }

    public void OnSprint(InputAction.CallbackContext callback)
    {
        // 대시/스프린트 추가할 경우 사용
        if (!_photonView.IsMine) return;

        if (callback.started || callback.performed)
        {
            _isSprinting = true;
            Debug.Log("Sprint ON");
        }
        else if (callback.canceled)
        {
            _isSprinting = false;
            Debug.Log("Sprint OFF");
        }
    }

    public void OnLook(InputAction.CallbackContext callback)
    {
        if (!_photonView.IsMine) return;

        Vector2 screenPos = callback.ReadValue<Vector2>();
        RotateTowardsScreenPosition(screenPos);
    }

    private void RotateTowardsScreenPosition(Vector2 screenPos)
    {
        Plane groundPlane = new Plane(Vector3.up, transform.position.y);
        Ray ray = _mainCamera.ScreenPointToRay(screenPos);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 lookTarget = ray.GetPoint(distance);
            Vector3 direction = lookTarget - transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 20f * Time.deltaTime);
            }
        }
    }

    private void Update()
    {
        if (!_photonView.IsMine) return;

        Vector3 move = new Vector3(_movement.x, 0, _movement.y);

        // 회전 (입력 방향이 있을 때만)
        //if (move.sqrMagnitude > 0.001f)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(move);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 30f * Time.deltaTime);
        //}

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
        if (_isSprinting)
        {
            // 매직넘버 : 임시 (PlayerStat 관련 미구현
            _controller.Move(move * 10f * Time.deltaTime);
        }
        else
        {
            _controller.Move(move * 5f * Time.deltaTime);
        }
    }
}