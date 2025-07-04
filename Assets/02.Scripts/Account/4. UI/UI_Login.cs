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
        UIManager.Instance.OpenPanel(EUIPanelType.Register);
    }

    public async void OnLoginButtonClicked()
    {
        string email = _emailInputField.text;
        string password = _passwordInputField.text;

        UIManager.Instance.ShowLoading(true);

        AccountResult result = await AccountManager.Instance.LoginAsync(email, password);

        UIManager.Instance.ShowLoading(false);

        if (result.Success)
        {
            UIManager.Instance.OpenPanel(EUIPanelType.BulletinBoard); // 성공 시 메인 UI로 전환
        }
        else
        {
            UIManager.Instance.ShowError(result.ErrorMessage);
            UIManager.Instance.OpenPanel(EUIPanelType.Login);
        }
    }
}
