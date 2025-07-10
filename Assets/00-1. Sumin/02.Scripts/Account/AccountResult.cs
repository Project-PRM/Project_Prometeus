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