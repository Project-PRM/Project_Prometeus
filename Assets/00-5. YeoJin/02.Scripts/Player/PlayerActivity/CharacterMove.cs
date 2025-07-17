using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(CharacterController))]
public class CharacterMove : MonoBehaviour
{
    private Vector2 _movement;
    private float _yVelocity = 0f;
    private bool _isSprinting = false;

    private const float GRAVITY = -9.81f;

    private Camera _mainCamera;

    private CharacterBehaviour _characterBehaviour;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _characterBehaviour = GetComponent<CharacterBehaviour>();
    }

    public void OnMove(InputAction.CallbackContext callback)
    {
        if (!_characterBehaviour.PhotonView.IsMine) return;

        if (callback.performed || callback.canceled)
        {
            _movement = callback.ReadValue<Vector2>();
            _characterBehaviour.Animator.SetFloat("Move", _movement.magnitude);
        }
    }

    public void OnSprint(InputAction.CallbackContext callback)
    {
        if (!_characterBehaviour.PhotonView.IsMine) return;

        if (callback.started || callback.performed)
        {
            _isSprinting = true;
        }
        else if (callback.canceled)
        {
            _isSprinting = false;
        }
    }

    public void OnLook(InputAction.CallbackContext callback)
    {
        if (!_characterBehaviour.PhotonView.IsMine) return;

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

    // CharacterBehaviour에서 매 프레임 호출
    public void Tick()
    {
        if (!_characterBehaviour.PhotonView.IsMine) return;

        Vector3 move = new Vector3(_movement.x, 0, _movement.y);

        // 중력
        if (_characterBehaviour.Controller.isGrounded)
        {
            _yVelocity = -1f;
        }
        else
        {
            _yVelocity += GRAVITY * Time.deltaTime;
        }

        move.y = _yVelocity;

        float speed = _isSprinting ? 10f : 5f;
        _characterBehaviour.Controller.Move(move * speed * Time.deltaTime);
    }
}
