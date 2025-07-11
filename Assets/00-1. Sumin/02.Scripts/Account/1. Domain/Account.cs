using Firebase.Firestore;

public class Account
{
    [FirestoreProperty] public string UserId { get; set; } // Firebase User ID입니다.
    [FirestoreProperty] public string Email { get; set; }
    [FirestoreProperty] public string Nickname { get; set; }

    private Account(string userId, string email, string nickname)
    {
        UserId = userId;
        Email = email;
        Nickname = nickname;
    }

    public static bool TryCreate(string userId, string email, string nickname, out Account account, out string errorMessage)
    {
        // var emailSpecification = new AccountEmailSpecification();
        // if (!emailSpecification.IsSatisfiedBy(email))
        // {
        //     account = null;
        //     errorMessage = emailSpecification.ErrorMessage;
        //     return false;
        // }
        //
        // var nicknameSpecification = new AccountNicknameSpecification();
        // if (!nicknameSpecification.IsSatisfiedBy(nickname))
        // {
        //     account = null;
        //     errorMessage = nicknameSpecification.ErrorMessage;
        //     return false;
        // }

        account = new Account(userId, email, nickname);
        errorMessage = null;
        return true;
    }

    public AccountDTO ToDto()
    {
        return new AccountDTO(this);
    }

    public void SetNickname(string newNickname, out string message)
    {
        var nicknameSpecification = new AccountNicknameSpecification();
        if (!nicknameSpecification.IsSatisfiedBy(newNickname))
        {
            message = nicknameSpecification.ErrorMessage;
            return;
        }

        Nickname = newNickname;
        message = "닉네임 변경에 성공했습니다.";
    }
}
