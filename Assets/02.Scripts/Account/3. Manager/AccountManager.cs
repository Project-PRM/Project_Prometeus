using System.Threading.Tasks;
using System;

public struct AccountResult
{
    public bool Success { get; }
    public string ErrorMessage { get; }

    public AccountResult(bool success, string errorMessage = null)
    {
        Success = success;
        ErrorMessage = errorMessage;
    }
}


public class AccountManager : Singleton<AccountManager>
{
    public event Action OnLoginSuccess;
    public event Action OnLogout;

    private AccountRepository _repository = new AccountRepository();

    public AccountDTO MyAccount => _repository.MyAccount;

    public async Task<AccountResult> RegisterAsync(string email, string nickname, string password)
    {
        AccountResult result = await _repository.RegisterAsync(email, nickname, password);

        return result;
    }

    public async Task<AccountResult> LoginAsync(string email, string password)
    {
        var (success, errorMessage) = await _repository.LoginAsync(email, password);

        if (success)
        {
            OnLoginSuccess?.Invoke();
        }

        return new AccountResult(success, errorMessage);
    }


    public void Logout()
    {
        _repository.Logout();
        OnLogout?.Invoke();
    }

    public async Task<bool> ChangeMyNicknameAsync(string newNickname, Action<string> onFail = null)
    {
        var (success, errorMessage) = await _repository.ChangeMyNicknameAsync(newNickname);
        if (!success)
        {
            onFail?.Invoke(errorMessage);
        }
        return success;
    }

    public async Task<string> GetUserNicknameWithEmail(string email)
    {
        string nickname = string.Empty;

        nickname = await _repository.GetNicknameByEmailAsync(email);

        return nickname; // 없으면 null 반환
    }
}
