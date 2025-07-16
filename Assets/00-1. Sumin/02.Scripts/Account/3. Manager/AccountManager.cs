using System.Threading.Tasks;
using System;
using System.Collections.Generic;

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
        var result = await _repository.LoginAsync(email, password);

        if (result.Success)
        {
            OnLoginSuccess?.Invoke();
        }

        return new AccountResult(result.Success, result.ErrorMessage);
    }

    public void Logout()
    {
        _repository.Logout();
        OnLogout?.Invoke();
    }

    public async Task<bool> ChangeMyNicknameAsync(string newNickname, Action<string> onFail = null)
    {
        var result = await _repository.ChangeMyNicknameAsync(newNickname);
        if (!result.Success)
        {
            onFail?.Invoke(result.ErrorMessage);
        }
        return result.Success;
    }

    public async Task<string> GetUidWithNickname(string nickname)
    {
        string uid = string.Empty;

        uid = await _repository.GetUidWithNicknameAsync(nickname);

        return uid; // 없으면 null 반환
    }
    public async Task<List<string>> GetUidsByNickname(string nickname)
    {
        List<string> uidList = new List<string>();

        uidList = await _repository.GetUidsWithNicknameAsync(nickname);

        return uidList; // 없으면 null 반환
        
       
    }
    public async Task<string> GetUserNicknameWithEmail(string email)
    {
        string nickname = string.Empty;

        nickname = await _repository.GetUserNicknameWithEmailAsync(email);

        return nickname; // 없으면 null 반환
    }
    public async Task<string> GetUserNicknameWithUid(string uid)
    {
        string nickname = string.Empty;

        nickname = await _repository.GetUserNicknameWithUidAsync(uid);

        return nickname; // 없으면 null 반환
    }
}
