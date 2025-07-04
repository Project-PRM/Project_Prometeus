using System.Text.RegularExpressions;

public class AccountEmailSpecification : ISpecification<string>
{
    private static readonly Regex _emailRegex =
        new(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", RegexOptions.Compiled);

    public string ErrorMessage { get; private set; }

    public bool IsSatisfiedBy(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ErrorMessage = "이메일은 비어있을 수 없습니다.";
            return false;
        }

        if (!_emailRegex.IsMatch(value))
        {
            ErrorMessage = "올바른 이메일 형식이 아닙니다.";
            return false;
        }

        return true;
    }
}