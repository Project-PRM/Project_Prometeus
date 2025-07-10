using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

public enum EUIPanelType
{
    Login,
    Register,
}

public class LoginUIManager : Singleton<LoginUIManager>
{
    [SerializeField] private TextMeshProUGUI _warningMessageText;
    
    // 타입 변경: GameObject -> UI_PopUp
    [SerializeField] private UI_PopUp _loginPanel;
    [SerializeField] private UI_PopUp _registerPanel;

    private Dictionary<EUIPanelType, UI_PopUp> _panels;

    protected override void Awake()
    {
        base.Awake();

        _panels = new Dictionary<EUIPanelType, UI_PopUp>
        {
            { EUIPanelType.Login, _loginPanel },
            { EUIPanelType.Register, _registerPanel },
        };
    }

    private void Start()
    {
        OpenPanel(EUIPanelType.Login);
    }

    /// <summary>
    /// 지정한 패널만 열고 나머지는 닫습니다.
    /// </summary>
    public void OpenPanel(EUIPanelType panelType)
    {
        ShowError(string.Empty);

        foreach (var pair in _panels)
        {
            if (pair.Value == null)
                continue;

            if (pair.Key == panelType) pair.Value.Show();
            else pair.Value.Hide();
        }

        //bool isLogin = !_registerPanel.gameObject.activeSelf && _loginPanel.gameObject.activeSelf;

        //_warningMessageText?.gameObject.SetActive(isLogin);
    }

    /// <summary>
    /// 모든 패널을 닫습니다.
    /// </summary>
    public void CloseAllPanels()
    {
        ShowError(string.Empty);

        foreach (var panel in _panels.Values)
        {
            if (panel == null)
                continue;
            
            panel.Hide();
        }
    }

    public void ShowError(string message)
    {
        _warningMessageText.text = message;
    }

    public void ShowLoading(bool flag)
    {
        ShowError("로딩중");
        //_panel_Loading.SetActive(flag);
    }
}