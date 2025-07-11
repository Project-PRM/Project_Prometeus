public class AccountDTO
{
    public readonly string UserId;
    public readonly string Email;
    public readonly string Nickname;

    public AccountDTO(Account account)
    {
        UserId = account.UserId;
        Email = account.Email;
        Nickname = account.Nickname;
    }
}