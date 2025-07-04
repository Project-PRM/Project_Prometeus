using System.Text.RegularExpressions;

public class AccountNicknameSpecification : ISpecification<string>
{
    private static readonly Regex _nicknameRegex =
        new(@"^(?!.*(바보|멍청이|운영자|김홍일))[가-힣A-Za-z]{2,12}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string ErrorMessage { get; private set; }

    public bool IsSatisfiedBy(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ErrorMessage = "닉네임은 비어있을 수 없습니다.";
            return false;
        }

        if (!_nicknameRegex.IsMatch(value))
        {
            ErrorMessage = "닉네임은 2~12자의 한글·영문만 가능하며, 부적절한 단어를 포함할 수 없습니다.";
            return false;
        }

        return true;
    }
}