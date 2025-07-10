using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    // 테스트
    private Vector2 _movement;
    private CharacterController _controller;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public void OnMove(InputAction.CallbackContext callback)
    {
        if(callback.performed || callback.canceled)
        {
            _movement = callback.ReadValue<Vector2>();
        }
    }

    private void Update()
    {
        Vector3 move = new Vector3(_movement.x, 0, _movement.y);
        _controller.Move(move * 5f * Time.deltaTime);
    }
}
