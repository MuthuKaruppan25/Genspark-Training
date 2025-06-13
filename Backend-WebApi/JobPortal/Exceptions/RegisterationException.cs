namespace JobPortal.Exceptions
{
    public class RegistrationException : Exception
    {
        public RegistrationException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }
}
