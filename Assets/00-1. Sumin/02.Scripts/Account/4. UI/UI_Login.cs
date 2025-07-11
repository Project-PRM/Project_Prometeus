using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Login : UI_PopUp
{
    [Header("# Scene")]
    [SerializeField] private string _nextSceneName = "Lobby_copied";

    [Header("# UIs")]
    [SerializeField] private TMP_InputField _emailInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private Button _toRegisterButton;
    [SerializeField] private Button _loginButton;


    [Header("테스트용 버튼")]
    public Button DummyLoginButton;

    private List<string> _dummyEmails = new List<string> {"test2@t.com", "test3@t.com", "test4@t.com", "test5@t.com", "test6@t.com", "test7@t.com", "test8@t.com", "test9@t.com", "test10@t.com", "test11@t.com", "test12@t.com", "test13@t.com", "test14@t.com", "test15@t.com" };
    private string _dummyPassword = "tttttt";
    
    public void OnDummyLoginButtonClick(int number)
    {
        _emailInputField.text = _dummyEmails[number];
        _passwordInputField.text = _dummyPassword;
        OnLoginButtonClicked();
    }
    protected override void Awake()
    {
        base.Awake();
        _toRegisterButton.onClick.AddListener(OnClickGoToRegister);
        _loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    public void OnClickGoToRegister()
    {
        LoginUIManager.Instance.OpenPanel(EUIPanelType.Register);
    }

    public async void OnLoginButtonClicked()
    {
        string email = _emailInputField.text;
        string password = _passwordInputField.text;

        LoginUIManager.Instance.ShowLoading(true);

        AccountResult result = await AccountManager.Instance.LoginAsync(email, password);

        LoginUIManager.Instance.ShowLoading(false);

        if (result.Success)
        {
            Debug.Log("로그인 성공");
            // TODO : 이거 바꾸셈
            SceneManager.LoadScene(1);
        }
        else
        {
            LoginUIManager.Instance.ShowError(result.ErrorMessage);
            //LoginUIManager.Instance.OpenPanel(EUIPanelType.Login);
        }
        EventManager.Broadcast(new DummyEvent(result.ErrorMessage));
    }
}
