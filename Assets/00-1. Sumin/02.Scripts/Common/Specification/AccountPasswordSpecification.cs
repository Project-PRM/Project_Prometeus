using System.Text.RegularExpressions;

public class AccountPasswordSpecification : ISpecification<string>
{
    private static readonly Regex _passwordRegex =
        new(@"^.{6,12}$", RegexOptions.Compiled);

    public string ErrorMessage { get; private set; }

    public bool IsSatisfiedBy(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ErrorMessage = "패스워드는 비어있을 수 없습니다.";
            return false;
        }

        //if (!_passwordRegex.IsMatch(value))
        //{
        //    ErrorMessage = "패스워드는 6자 이상 12자 이하여야 합니다.";
        //    return false;
        //}

        return true;
    }
}