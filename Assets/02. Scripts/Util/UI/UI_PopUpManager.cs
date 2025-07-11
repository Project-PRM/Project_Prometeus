using System.Collections.Generic;
using UnityEngine;

public class UI_PopUpManager : Singleton<UI_PopUpManager>
{
    private Stack<UI_PopUp> _popUps = new Stack<UI_PopUp>();

    private void Update()
    {
        // 키 입력으로 팝업을 닫는 기능 추가
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HidePopUp();
        }
    }

    public void ShowPopUp(UI_PopUp popUp)
    {
        if (_popUps.Count > 0)
        {
            _popUps.Peek().Hide();
        }
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