using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Register : UI_PopUp
{
    [Header ("# UIs")]
    [SerializeField] private TMP_InputField _emailInputField;
    [SerializeField] private TMP_InputField _nicknameInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private TMP_InputField _passwordCheckInputField;
    [SerializeField] private Button _registerButton;
    [SerializeField] private Button _toLoginButton;

    protected override void Awake()
    {
        base.Awake();
        _registerButton.onClick.AddListener(OnClickRegisterButton);
        _toLoginButton.onClick.AddListener(OnClickRegisterPanelExit);
    }

    public void OnClickRegisterPanelExit()
    {
        LoginUIManager.Instance.OpenPanel(EUIPanelType.Login);
    }

    public async void OnClickRegisterButton()
    {
        string email = _emailInputField.text;
        string nickname = _nicknameInputField.text;
        string password = _passwordInputField.text;
        string passwordCheck = _passwordCheckInputField.text;

        if (string.IsNullOrWhiteSpace(email))
        {
            LoginUIManager.Instance.ShowError("이메일를 입력해주세요.");
            return;
        }
        else if (string.IsNullOrWhiteSpace(nickname))
        {
            LoginUIManager.Instance.ShowError("닉네임을 입력해주세요.");
            return;
        }
        else if (string.IsNullOrWhiteSpace(password))
        {
            LoginUIManager.Instance.ShowError("비밀번호를 입력해주세요.");
            return;
        }
        else if (string.IsNullOrWhiteSpace(passwordCheck))
        {
            LoginUIManager.Instance.ShowError("비밀번호 확인을 입력해주세요.");
            return;
        }
        else if (password != passwordCheck)
        {
            LoginUIManager.Instance.ShowError("비밀번호와 확인이 일치하지 않습니다.");
            return;
        }
        
        AccountResult result = await AccountManager.Instance.RegisterAsync(email, nickname, password);
        if (result.Success)
        {
            Debug.Log($"회원가입 성공");
            LoginUIManager.Instance.OpenPanel(EUIPanelType.Login);
        }
        else
        {
            LoginUIManager.Instance.ShowError(result.ErrorMessage);
        }
    }
}