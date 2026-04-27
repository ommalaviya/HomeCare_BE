namespace Shared.HomeCare.Exceptions
{
    public class DuplicateRecordException : Exception
    {
        public DuplicateRecordException(string message) : base(message) { }
    }
}
