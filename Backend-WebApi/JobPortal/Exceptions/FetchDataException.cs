namespace JobPortal.Exceptions
{
    public class FetchDataException : Exception
    {
        public FetchDataException(string message)
            : base(message) { }
        public FetchDataException(string message, Exception innerException)
    : base(message, innerException) { }
    }
}
