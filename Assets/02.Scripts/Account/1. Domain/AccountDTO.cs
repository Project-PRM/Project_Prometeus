public class AccountDTO
{
    public readonly string Email;
    public readonly string Nickname;

    public AccountDTO(Account account)
    {
        Email = account.Email;
        Nickname = account.Nickname;
    }
}