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
            SceneManager.LoadScene(_nextSceneName);
        }
        else
        {
            LoginUIManager.Instance.ShowError(result.ErrorMessage);
            //LoginUIManager.Instance.OpenPanel(EUIPanelType.Login);
        }
        EventManager.Broadcast(new DummyEvent(result.ErrorMessage));
    }
}
