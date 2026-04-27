namespace Shared.HomeCare.Exceptions
{
    public class AccountInactiveException : Exception
    {
        public AccountInactiveException(string message) : base(message) { }
    }
}