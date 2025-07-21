using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_PopUpManager : Singleton<UI_PopUpManager>
{
    private Stack<UI_PopUp> _popUps = new Stack<UI_PopUp>();

    [SerializeField] private PlayerInput _playerInput; // PlayerInput 컴포넌트 연결 필요

    private void OnEnable()
    {
        // "Cancel" 액션에 이벤트 연결
        
        _playerInput.actions["Cancel"].performed += OnCancel;
    }

    private void OnDisable()
    {
        _playerInput.actions["Cancel"].performed -= OnCancel;
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        HidePopUp();
    }

    private bool ContainsPopUp(UI_PopUp popUp)
    {
        foreach (var p in _popUps)
        {
            if (p == popUp) return true;
        }
        return false;
    }

    public void ShowPopUp(UI_PopUp popUp)
    {
        if (ContainsPopUp(popUp))
            return;

        _popUps.Push(popUp);
        popUp.Show();
    }

    public void HidePopUp()
    {
        if (_popUps.Count > 0)
        {
            UI_PopUp popUp = _popUps.Pop();
            popUp.Hide();
            if (_popUps.Count > 0)
            {
                _popUps.Peek().Show();
            }
        }
    }
}
