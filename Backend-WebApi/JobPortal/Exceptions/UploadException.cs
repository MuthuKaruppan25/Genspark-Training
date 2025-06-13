namespace JobPortal.Exceptions
{
    public class UploadException : Exception
    {
        public UploadException(string message)
            : base(message) { }
        public UploadException(string message, Exception innerException)
    : base(message, innerException) { }
    }
}
