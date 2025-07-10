using TMPro;
using UnityEngine;

public class UI_Login : UI_PopUp
{
    [SerializeField] private TMP_InputField _emailInputField;
    [SerializeField] private TMP_InputField _passwordInputField;

    public override void Show() { base.Show(); }
    public override void Hide() { base.Hide(); }

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
            LoginUIManager.Instance.OpenPanel(EUIPanelType.BulletinBoard); // 성공 시 메인 UI로 전환
        }
        else
        {
            LoginUIManager.Instance.ShowError(result.ErrorMessage);
            LoginUIManager.Instance.OpenPanel(EUIPanelType.Login);
        }
    }
}
